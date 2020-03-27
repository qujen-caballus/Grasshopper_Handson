using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Magnet_handson
{
    public class Magnet_handsonInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Magnethandson";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("b60c2a34-d543-4228-8fb5-7cb8560e18e4");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
