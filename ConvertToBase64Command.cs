using System;
using System.Management.Automation;

namespace PSTextUtils
{
    [Cmdlet(VerbsData.ConvertTo, "Base64String")]
    [OutputType(typeof(string))]
    public class ConvertToBase64Command : Cmdlet
    {
        // Declare the parameters for the cmdlet.
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [Alias("InputString", "String", "Text")]
        public string InputObject { get; set; }


        [Parameter]
        [ArgumentCompleter(typeof(EncodingCompleter))]
        [EncodingTransform]
        public System.Text.Encoding Encoding { get; set; } = EncodingTransformAttribute.StandardEncoding["UTF8"];


        //This is the "Process {}" block of a Powershell script
        protected override void ProcessRecord()
        {
            //WriteObject("Hello " + name + "!");

            byte[] bytes = Encoding.GetBytes(InputObject);
            WriteObject(Convert.ToBase64String(bytes), true);

        }
    }
}
