using System;
using System.Collections.Generic;

namespace Mix.Cms.Lib.Models.Cms
{
    public partial class MixDatabase
    {
        public MixDatabase()
        {
            MixAttributeFieldAttributeSet = new HashSet<MixDatabaseColumn>();
            MixAttributeFieldReference = new HashSet<MixDatabaseColumn>();
            MixAttributeSetData = new HashSet<MixDatabaseData>();
            MixAttributeSetReference = new HashSet<MixDatabaseRelationship>();
            MixRelatedAttributeSet = new HashSet<MixRelatedAttributeSet>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FormTemplate { get; set; }
        public string EdmTemplate { get; set; }
        public string EdmSubject { get; set; }
        public string EdmFrom { get; set; }
        public bool? EdmAutoSend { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int Priority { get; set; }
        public MixEnums.MixContentStatus Status { get; set; }

        public virtual ICollection<MixDatabaseColumn> MixAttributeFieldAttributeSet { get; set; }
        public virtual ICollection<MixDatabaseColumn> MixAttributeFieldReference { get; set; }
        public virtual ICollection<MixDatabaseData> MixAttributeSetData { get; set; }
        public virtual ICollection<MixDatabaseRelationship> MixAttributeSetReference { get; set; }
        public virtual ICollection<MixRelatedAttributeSet> MixRelatedAttributeSet { get; set; }
    }
}
