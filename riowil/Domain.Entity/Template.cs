﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Template
	{
		public int Id { get; set; }
		public IEnumerable<Cluster> Clusters { get; set; }
		public int[] Value { get; set; }
	}
}
