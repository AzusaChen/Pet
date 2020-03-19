using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace prjNewPet.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(decimal? y, string label)
        {
            this.Y = y;
            this.Label = label;
        }

        public DataPoint(double x, decimal? y)
        {
            this.X = x;
            this.Y = y;
        }


        public DataPoint(double x, decimal? y, string label)
        {
            this.X = x;
            this.Y = y;
            this.Label = label;
        }

        public DataPoint(double x, decimal? y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public DataPoint(double x, decimal? y, double z, string label)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Label = label;
        }


        //Explicitly setting the name to be used while serializing to JSON. 
        [DataMember(Name = "label")]
        public string Label = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<decimal> Y = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public Nullable<double> X = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "z")]
        public Nullable<double> Z = null;
    }
}