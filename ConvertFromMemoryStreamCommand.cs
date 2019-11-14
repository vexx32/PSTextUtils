using System;
using System.IO;
using System.Management.Automation;

namespace PSTextUtils {


    [Cmdlet(VerbsData.ConvertFrom,"MemoryStream")]
    [OutputType(typeof(String))]

    public class ConvertFromMemoryStreamCommand : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public byte[] InputObject { get; set; }

        protected override void ProcessRecord()
        {
            MemoryStream stream = new MemoryStream(InputObject);
            StreamReader reader = new StreamReader(stream);

            string Output = reader.ReadToEnd();

            WriteObject(Output);
        }

    }

}