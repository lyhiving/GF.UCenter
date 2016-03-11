using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database;
using UCenter.Common.Database.Entities;

namespace UCenter.Test
{
    [DatabaseTableName("test")]
    public class TestDatabaseEntity : BaseEntity<TestDatabaseEntity>
    {
        [Column(IsKey = true, Length = 100)]
        public string Id { get; set; }

        [Column(Length = 100)]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
