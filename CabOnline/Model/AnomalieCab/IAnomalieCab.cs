namespace CabOnline.Model.AnomalieCab
{
    internal interface IAnomalieCab
    {
        string Libell�Court { get;}
        string Libell�Long { get; }
        TypeAnomalieCab Type { get;  }
        bool N�cessiteAction { get;  }
        bool EstIncompatible { get; }
        string ValeurParution { get;  }
        string ValeurCab { get;  }
    }
}