using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAffinity
{
    public class ProcessAffinity
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr CreateJobObject(IntPtr a, string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr CreateJobObject([In] ref SECURITY_ATTRIBUTES lpJobAttributes, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, UInt32 cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetCurrentThread();

        [DllImport("kernel32", SetLastError = true)]
        private static extern Boolean SetThreadGroupAffinity(IntPtr hThread, ref _GROUP_AFFINITY GroupAffinity, ref _GROUP_AFFINITY PreviousGroupAffinity);

        [DllImport("kernel32", SetLastError = true)]
        private static extern uint GetLastError(out uint pulErrCode, char[] strBuf, ref ushort pusBufLen);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint GetLastError();

        [DllImport("advapi32.dll", EntryPoint = "SetSecurityInfo", CallingConvention = CallingConvention.Winapi, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern /*DWORD*/ uint SetSecurityInfoByHandle(SafeHandle handle, /*DWORD*/ uint objectType, /*DWORD*/ uint securityInformation, byte[] owner, byte[] group, byte[] dacl, byte[] sacl);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryInformationJobObject(IntPtr hJob, JobObjectInfoClass JobObjectInformationClass, ref JOBOBJECT_BASIC_PROCESS_ID_LIST lpJobObjectInfo, int cbJobObjectInfoLength, IntPtr lpReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt16 GetActiveProcessorGroupCount();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNumaHighestNodeNumber(out uint count);

        private static IntPtr handle;
        private static AppSetting appSetting = new AppSetting();

        public static bool Setup(out string result)
        {
            
            if (!appSetting.EnableProcessAffinity)
            {
                result = $"{AffinityStatus.NotEnabled}, EnableProcessAffinity=F";
                return true;
            }

            if (string.IsNullOrWhiteSpace(appSetting.ProcessAffinity))
            {
                result = $"{AffinityStatus.NotEnabled}, ProcessAffinity is empty";
                return true;
            }

            if (!TryGetAffinityRanges(appSetting.ProcessAffinity, out Dictionary<int, List<int>> cpuGroupCoresDict, out result))
            {
                return false;
            }

            if (!IsValidInput(appSetting.ProcessAffinity, cpuGroupCoresDict, out ushort groupId, out int[] cores, out result))
            {
                return false;
            }

            if (!TryGetCpuMask(appSetting.ProcessAffinity, cores, out long cpuMask, out result))
            {
                return false;
            }

            // if (!TrySetThreadProcessorAffinity(groupId, cpuMask, out result))
            // {
            // 	return false;
            // }

            if (!TrySetProcessorAffinity(cpuMask, out result))
            {
                return false;
            }

            if (!TrySetInformationJobObject(groupId, cpuMask, out result))
            {
                return false;
            }
            return true;
        }

        public static bool TrySetProcessorAffinity(long cpuMask, out string result)
        {
            result = string.Empty;

            try
            {
                if (cpuMask < 0)
                {
                    result = $"{AffinityStatus.Fail}, cpuMask cannot be a negative value, cpuMask:{cpuMask}";
                    return false;
                }
                try
                {
                    Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)cpuMask;
                    result = $"{AffinityStatus.InProgress}, TrySetProcessorAffinity succeeded, cpuMask:{cpuMask}";
                    return true;

                }
                catch (Exception ex)
                {
                    Process.GetCurrentProcess().Refresh();
                    result = $"{AffinityStatus.InProgress}, TrySetProcessorAffinity failed, {ex.Message}, cpuMask:{cpuMask}, process.ProcessorAffinity:{(long)Process.GetCurrentProcess().ProcessorAffinity}";
                    return false;
                }
            }

            catch (Exception ex)
            {
                result = $"{AffinityStatus.Fail}, TrySetProcessorAffinity failed, {ex.Message}, cpuMask:{cpuMask}, ErrorCode:{(ex is Win32Exception ? (ex as Win32Exception).NativeErrorCode : ex.HResult)}";
                return false;
            }
        }

        public static bool TrySetThreadProcessorAffinity(ushort groupId, long cpuMask, out string result)
        {
            try
            {
                var hThread = GetCurrentThread();
                var previousAffinity = new _GROUP_AFFINITY { Reserved = new ushort[3] };
                var newAffinity = new _GROUP_AFFINITY
                {
                    Group = groupId,
                    Mask = new UIntPtr((ulong)cpuMask),
                    Reserved = new ushort[3]
                };

                if (!SetThreadGroupAffinity(hThread, ref newAffinity, ref previousAffinity))
                {
                    result = $"{AffinityStatus.Fail}, SetThreadProcessorAffinity failed, newAffinity:{newAffinity.Group}::{newAffinity.Mask}, previousAffinity:{previousAffinity.Group}::{previousAffinity.Mask}, groupId:{groupId}, cpuMask:{cpuMask}";
                    return false;
                }

                result = $"{AffinityStatus.Success}, SetThreadProcessorAffinity succeeded, newAffinity:{newAffinity.Group}::{newAffinity.Mask}, previousAffinity:{previousAffinity.Group}::{previousAffinity.Mask}, groupId:{groupId}, cpuMask:{cpuMask}";
                return true;
            }
            catch (Exception ex)
            {
                result = $"{AffinityStatus.Fail}, SetThreadProcessorAffinity failed, {ex.Message}, ErrorCode:{(ex is Win32Exception ? (ex as Win32Exception).NativeErrorCode : ex.HResult)}, groupId:{groupId}, cpuMask:{cpuMask}";
                return false;
            }
        }

        public static bool TrySetInformationJobObject(ushort groupId, long cpuMask, out string result)
        {
            result = string.Empty;
            try
            {
                _GROUP_AFFINITY newAffinity = new _GROUP_AFFINITY
                {
                    Group = groupId,
                    Mask = new UIntPtr((ulong)cpuMask),
                    Reserved = new ushort[3]
                };

                SECURITY_ATTRIBUTES lpJobAttributes = new();
                handle = CreateJobObject(ref lpJobAttributes, null);

                if (!AddProcess(Environment.ProcessId))
                {
                    result = $"{AffinityStatus.Fail}, Cannot add current process {Environment.ProcessId} to the Job, groupId:{groupId}, cpuMask:{cpuMask}";
                    return false;
                }

                int length = Marshal.SizeOf(typeof(_GROUP_AFFINITY));
                //int length = Marshal.SizeOf(newAffinity);
                IntPtr newAffinityPtr = Marshal.AllocHGlobal(length);
                Marshal.StructureToPtr(newAffinity, newAffinityPtr, false);

                try
                {
                    if (SetInformationJobObject(handle, JobObjectInfoType.JobObjectGroupInformationEx, newAffinityPtr, (uint)length))
                    {
                        result = $"{AffinityStatus.Success}, TrySetInformationJobObject succeeded, ProcessId:{Environment.ProcessId}, ProcessAffinity:{appSetting.ProcessAffinity}, newAffinity:{newAffinity.Group}::{newAffinity.Mask}, groupId:{groupId}, cpuMask:{cpuMask}";
                        return true;
                    }

                    result = $"{AffinityStatus.Fail}, TrySetInformationJobObject failed, ProcessAffinity:{appSetting.ProcessAffinity} failed, ErrorCode:{(int)GetLastError()}, groupId:{groupId}, cpuMask:{cpuMask}";
                    return false;
                }
                catch (Exception ex)
                {
                    Process.GetCurrentProcess().Refresh();
                    result = $"{AffinityStatus.InProgress}, TrySetInformationJobObject failed, {ex.Message}, groupId:{groupId}, cpuMask:{cpuMask}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = $"{AffinityStatus.Fail}, TrySetInformationJobObject failed, {ex.Message}, ErrorCode:{(ex is Win32Exception ? (ex as Win32Exception).NativeErrorCode : ex.HResult)}, groupId:{groupId}, cpuMask:{cpuMask}";
                return false;
            }
        }


        public static bool IsValidInput(string processAffinity, Dictionary<int, List<int>> cpuGroupCoreDict, out ushort groupId, out int[] cores, out string result)
        {
            groupId = 0;
            cores = null;
            result = string.Empty;
            try
            {
                if (cpuGroupCoreDict == null || cpuGroupCoreDict.Keys.Count == 0 || cpuGroupCoreDict.Values.Count == 0)
                {
                    result = $"{AffinityStatus.Fail}, Invalid ProcessAffinity Setting defined [{nameof(processAffinity)}={processAffinity}]";
                    return false;
                }

                if (cpuGroupCoreDict.Keys.Count > 1)
                {
                    result = $"{AffinityStatus.Fail}, Cross CPU group ProcessAffinity [{nameof(processAffinity)}={processAffinity}] not supported yet";
                    return false;
                }

                int highestNumeNodeNumber = GetNumaHighestNodeNumber();
                int maxCpuGroupConfigured = cpuGroupCoreDict.Keys.Max();
                if (maxCpuGroupConfigured > highestNumeNodeNumber)
                {
                    result = $"{AffinityStatus.Fail}, Invalid CpuGroup {maxCpuGroupConfigured}, highest CpuGroup value supported on this computer is {highestNumeNodeNumber}";
                    return false;
                }

                if (cpuGroupCoreDict.ContainsKey(0))
                {
                    groupId = 0;
                    cores = cpuGroupCoreDict[0].ToArray();
                }
                else if (cpuGroupCoreDict.ContainsKey(1))
                {
                    groupId = 1;
                    cores = cpuGroupCoreDict[1].ToArray();
                }
                else
                {
                    groupId = (ushort)cpuGroupCoreDict.First().Key;
                    result = $"{AffinityStatus.Fail}, Configured CpuGroupId [{nameof(groupId)}={groupId}] not supported yet";
                    return false;
                }

                if (groupId < 0)
                {
                    result = $"{AffinityStatus.Fail}, Invalid CpuGroupId [{nameof(groupId)}={groupId}]";
                    return false;
                }

                if (cores == null)
                {
                    result = $"{AffinityStatus.Fail}, Invalid core range [{nameof(cores)}={cores}]";
                    return false;
                }

                if (cores.Length == 0)
                {
                    result = $"{AffinityStatus.Fail}, At least one core must be specified in ProcessorAffinity";
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = $"{AffinityStatus.Fail}, {ex.Message}";
                return false;
            }
            result = $"{AffinityStatus.InProgress}, Validation passed";
            return true;
        }

        public static bool TryGetCpuMask(string processAffinity, int[] cores, out long cpuMask, out string result)
        {
            cpuMask = 0;
            try
            {
                if (cores.Min() < 0 || cores.Max() >= Environment.ProcessorCount)
                {
                    result = $"{AffinityStatus.Fail}, Invalid core number(s) found in the range {processAffinity}. Core number must be in between 0 and {Environment.ProcessorCount - 1}";
                    return false;
                }

                foreach (int core in cores)
                {
                    cpuMask |= 1L << core;
                }
            }
            catch (Exception ex)
            {
                result = $"{AffinityStatus.Fail}, Obtaining CpuMask failed, Error={ex.Message}";
                return false;
            }
            result = $"{AffinityStatus.InProgress}";
            return true;
        }

        private static bool AddProcess(int processId) => AddProcess(Process.GetProcessById(processId).Handle);

        private static bool AddProcess(IntPtr processHandle) => AssignProcessToJobObject(handle, processHandle);

        public static bool TryGetAffinityRanges(string affinityRange, out Dictionary<int, List<int>> cpuGroupCoresDict, out string result)
        {
            //0:0,1-3,4,5;1:6,7,8-10,15
            result = string.Empty;
            bool hasValue = false;
            cpuGroupCoresDict = new Dictionary<int, List<int>>();
            try
            {
                string[] ranges = affinityRange.Split(new char[] { ';' });

                foreach (string range in ranges)
                {
                    string[] groupCore = range.Split(':', StringSplitOptions.RemoveEmptyEntries);

                    if (int.TryParse(groupCore[0], out int group))
                    {
                        cpuGroupCoresDict.Add(group, new List<int>());
                        string[] cores = groupCore[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string core in cores)
                        {
                            if (core.Contains('-'))
                            {
                                string[] cpuRange = core.Split('-', StringSplitOptions.RemoveEmptyEntries);
                                if (int.TryParse(cpuRange[0], out int from) && int.TryParse(cpuRange[1], out int to))
                                {
                                    if (cpuGroupCoresDict.TryGetValue(group, out List<int> cpus))
                                    {
                                        cpus.AddRange(Enumerable.Range(from, to - from + 1));
                                    }
                                    else
                                    {
                                        cpuGroupCoresDict[group] = Enumerable.Range(from, to - from + 1) as List<int>;
                                    }
                                    hasValue = true;
                                }
                            }
                            else
                            {
                                if (int.TryParse(core, out int cpu))
                                {
                                    if (cpuGroupCoresDict.TryGetValue(group, out List<int> cpus))
                                    {
                                        cpus.Add(cpu);
                                    }
                                    else
                                    {
                                        cpuGroupCoresDict[group] = new List<int>() { cpu };
                                    }
                                    hasValue = true;
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                result = $"{AffinityStatus.Fail}, Failed during extracting affinity range from ProcessAffinity setting i.e {affinityRange}, Error: {ex.Message}";
                return false;
            }
            if (!hasValue)
            {
                result = $"{AffinityStatus.Fail}, Failed during extracting affinity range from ProcessAffinity setting i.e {affinityRange}";
            }
            return hasValue;
        }

        private static int GetNumaHighestNodeNumber()
        {
            uint numaHighestNodeNumber = 0;
            try
            {
                GetNumaHighestNodeNumber(out numaHighestNodeNumber);
            }
            catch (Exception)
            {
                return -1;
            }

            return (int)numaHighestNodeNumber; // Node number start at 0
        }

        public static void Close()
        {
            CloseHandle(handle);
            handle = IntPtr.Zero;
        }
    }

    public enum AffinityStatus
    {
        NotEnabled,
        Fail,
        Success,
        InProgress
    }

    public enum JobObjectInfoType
    {
        AssociateCompletionPortInformation = 7,
        BasicLimitInformation = 2,
        BasicUIRestrictions = 4,
        EndOfJobTimeInformation = 6,
        ExtendedLimitInformation = 9,
        SecurityLimitInformation = 5,
        GroupInformation = 11,
        JobObjectGroupInformationEx = 14
    }

    public enum JobObjectInfoClass
    {
        JobObjectBasicAccountingInformation = 1,
        JobObjectBasicLimitInformation = 2,
        JobObjectBasicProcessIdList = 3,
        JobObjectBasicUIRestrictions = 4,
        JobObjectSecurityLimitInformation = 5,
        JobObjectEndOfJobTimeInformation = 6,
        JobObjectAssociateCompletionPortInformation = 7,
        JobObjectBasicAndIoAccountingInformation = 8,
        JobObjectExtendedLimitInformation = 9,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct _GROUP_AFFINITY
    {
        public UIntPtr Mask;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Group;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U2)]
        public ushort[] Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IO_COUNTERS
    {
        public UInt64 ReadOperationCount;
        public UInt64 WriteOperationCount;
        public UInt64 OtherOperationCount;
        public UInt64 ReadTransferCount;
        public UInt64 WriteTransferCount;
        public UInt64 OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public Int64 PerProcessUserTimeLimit;
        public Int64 PerJobUserTimeLimit;
        public UInt32 LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public UInt32 ActiveProcessLimit;
        public UIntPtr Affinity;
        public UInt32 PriorityClass;
        public UInt32 SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public UInt32 nLength;
        public IntPtr lpSecurityDescriptor;
        public Int32 bInheritHandle;
    }

    public struct JOBOBJECT_BASIC_PROCESS_ID_LIST
    {
        public int NumberOfAssignedProcesses;
        public int NumberOfProcessIdsInList;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public int[] ProcessIdList;
    }
}
