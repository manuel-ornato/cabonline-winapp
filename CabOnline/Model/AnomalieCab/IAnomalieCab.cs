namespace CabOnline.Model.AnomalieCab
{
    internal interface IAnomalieCab
    {
        string LibelléCourt { get;}
        string LibelléLong { get; }
        TypeAnomalieCab Type { get;  }
        bool NécessiteAction { get;  }
        bool EstIncompatible { get; }
        string ValeurParution { get;  }
        string ValeurCab { get;  }
    }
}