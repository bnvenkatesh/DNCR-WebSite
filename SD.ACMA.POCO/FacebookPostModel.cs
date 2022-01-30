using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SD.ACMA.POCO
{
    [Serializable]
    public class FacebookPostModel
    {

        public string Id;
        public ModelFrom From;
        public string Message;
        public string Picture;
        public string Link;
        public string Source;
        public string Name;
        public ModelProperty[] Properties;
        public string Icon;
        public ModelPrivacy Privacy;
        public object Place;
        public string Type;
        public string Status_type;
        public string Object_id;
        public ModelApplication Application;
        public string Created_time;
        public string Updated_time;
        public ModelComments Comments;

        //Empty Constructor
        public FacebookPostModel() { }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static FacebookPostModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<FacebookPostModel>(json);
        }
    }


    [Serializable]
    public class ModelFrom
    {

        public string Category;
        public string Name;
        public string Id;

        //Empty Constructor
        public ModelFrom() { }

    }


    [Serializable]
    public class ModelProperty
    {

        public string Name;
        public string Text;

        //Empty Constructor
        public ModelProperty() { }

    }


    [Serializable]
    public class ModelPrivacy
    {

        public string Value;

        //Empty Constructor
        public ModelPrivacy() { }

    }


    [Serializable]
    public class ModelApplication
    {

        public string Name;
        public string Namespace;
        public string Id;

        //Empty Constructor
        public ModelApplication() { }

    }


    [Serializable]
    public class ModelFrom2
    {

        public string Category;
        public string Name;
        public string Id;

        //Empty Constructor
        public ModelFrom2() { }

    }


    [Serializable]
    public class ModelDatum
    {

        public string Id;
        public ModelFrom2 From;
        public string Message;
        public bool Can_remove;
        public string Created_time;
        public int Like_count;
        public bool User_likes;

        //Empty Constructor
        public ModelDatum() { }

    }


    [Serializable]
    public class ModelCursors
    {

        public string After;
        public string Before;

        //Empty Constructor
        public ModelCursors() { }

    }


    [Serializable]
    public class ModelPaging
    {

        public ModelCursors Cursors;

        //Empty Constructor
        public ModelPaging() { }

    }


    [Serializable]
    public class ModelComments
    {

        public ModelDatum[] Data;
        public ModelPaging Paging;

        //Empty Constructor
        public ModelComments() { }

    }
}
