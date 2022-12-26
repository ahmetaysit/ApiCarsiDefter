using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{
    [Table("Settings")]
    public partial class Settings
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string SettingKey { get; set; }

        [StringLength(50)]
        public string Settingvalue { get; set; }

        public bool? IsActive { get; set; }
    }
}
