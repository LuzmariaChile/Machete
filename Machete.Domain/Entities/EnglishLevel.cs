﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Machete.Domain.Resources;
namespace Machete.Domain
{
    public class EnglishLevel
    {
        public byte ID { get; set; }
        public string englishlevel { get; set; }
    }
}