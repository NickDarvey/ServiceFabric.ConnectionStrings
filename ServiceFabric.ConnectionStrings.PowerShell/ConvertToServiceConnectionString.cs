using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    [Cmdlet(VerbsData.ConvertTo, "ServiceConnectionString")]
    public class ConvertToServiceConnectionString : Cmdlet
    {

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "A hashtable of service configuration values")]
        public Hashtable[] ServiceConfiguration { get; set; }


        protected override void BeginProcessing()
        {
            foreach(var configuration in ServiceConfiguration)
            {
                var parts = configuration.Cast<DictionaryEntry>()
                    .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());

                var connectionString = ServiceConnectionString.Parse(parts);

                WriteObject(connectionString.ToString());
            }
        }
    }
}
