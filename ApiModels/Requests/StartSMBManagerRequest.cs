﻿using ApiModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.Requests
{
	public class StartSMBmanagerRequest
	{
		public string Name { get; set; }
        public string NamedPipe { get; set; }
        public ManagerType managertype { get; set; }
        public ConnectionMode connectionMode { get; set; } // always means direction of parent -> child
        public string ConnectionAddress { get; set; } // address a tcp client connects to, enabled on reverse

        public enum ManagerType
        {
            http, https, tcp, smb
        }
        public enum ConnectionMode
        {
            bind,
            reverse
        }
    }
  
}
