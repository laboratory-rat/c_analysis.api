using MRMigrationMaster.Infrastructure.Attr;
using MRMigrationMaster.Infrastructure.Component;
using MRMigrationMaster.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeneralMigrations.Collection
{
    [MigrationAttr("Init migration", "04.04.2019")]
    class InitMigration : Migration, IMigration
    {

        public override async Task Action()
        {

        }
    }
}
