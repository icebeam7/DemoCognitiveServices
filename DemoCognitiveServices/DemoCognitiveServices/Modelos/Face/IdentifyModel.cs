using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCognitiveServices.Modelos.Face
{
    public class IdentifyModel
    {
        public List<string> FaceIds { get; set; }
        public string PersonGroupId { get; set; }
        public int MaxNumOfCandidatesReturned { get; set; }
        public double ConfidenceThreshold { get; set; }
    }
}
