//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[State]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class State : IHavePrimaryKey
    {
        public static readonly StateAlabama Alabama = StateAlabama.Instance;
        public static readonly StateAlaska Alaska = StateAlaska.Instance;
        public static readonly StateArizona Arizona = StateArizona.Instance;
        public static readonly StateArkansas Arkansas = StateArkansas.Instance;
        public static readonly StateCalifornia California = StateCalifornia.Instance;
        public static readonly StateColorado Colorado = StateColorado.Instance;
        public static readonly StateConnecticut Connecticut = StateConnecticut.Instance;
        public static readonly StateDelaware Delaware = StateDelaware.Instance;
        public static readonly StateFlorida Florida = StateFlorida.Instance;
        public static readonly StateGeorgia Georgia = StateGeorgia.Instance;
        public static readonly StateHawaii Hawaii = StateHawaii.Instance;
        public static readonly StateIdaho Idaho = StateIdaho.Instance;
        public static readonly StateIllinois Illinois = StateIllinois.Instance;
        public static readonly StateIndiana Indiana = StateIndiana.Instance;
        public static readonly StateIowa Iowa = StateIowa.Instance;
        public static readonly StateKansas Kansas = StateKansas.Instance;
        public static readonly StateKentucky Kentucky = StateKentucky.Instance;
        public static readonly StateLouisiana Louisiana = StateLouisiana.Instance;
        public static readonly StateMaine Maine = StateMaine.Instance;
        public static readonly StateMaryland Maryland = StateMaryland.Instance;
        public static readonly StateMassachusetts Massachusetts = StateMassachusetts.Instance;
        public static readonly StateMichigan Michigan = StateMichigan.Instance;
        public static readonly StateMinnesota Minnesota = StateMinnesota.Instance;
        public static readonly StateMississippi Mississippi = StateMississippi.Instance;
        public static readonly StateMissouri Missouri = StateMissouri.Instance;
        public static readonly StateMontana Montana = StateMontana.Instance;
        public static readonly StateNebraska Nebraska = StateNebraska.Instance;
        public static readonly StateNevada Nevada = StateNevada.Instance;
        public static readonly StateNewHampshire NewHampshire = StateNewHampshire.Instance;
        public static readonly StateNewJersey NewJersey = StateNewJersey.Instance;
        public static readonly StateNewMexico NewMexico = StateNewMexico.Instance;
        public static readonly StateNewYork NewYork = StateNewYork.Instance;
        public static readonly StateNorthCarolina NorthCarolina = StateNorthCarolina.Instance;
        public static readonly StateNorthDakota NorthDakota = StateNorthDakota.Instance;
        public static readonly StateOhio Ohio = StateOhio.Instance;
        public static readonly StateOklahoma Oklahoma = StateOklahoma.Instance;
        public static readonly StateOregon Oregon = StateOregon.Instance;
        public static readonly StatePennsylvania Pennsylvania = StatePennsylvania.Instance;
        public static readonly StateRhodeIsland RhodeIsland = StateRhodeIsland.Instance;
        public static readonly StateSouthCarolina SouthCarolina = StateSouthCarolina.Instance;
        public static readonly StateSouthDakota SouthDakota = StateSouthDakota.Instance;
        public static readonly StateTennessee Tennessee = StateTennessee.Instance;
        public static readonly StateTexas Texas = StateTexas.Instance;
        public static readonly StateUtah Utah = StateUtah.Instance;
        public static readonly StateVermont Vermont = StateVermont.Instance;
        public static readonly StateVirginia Virginia = StateVirginia.Instance;
        public static readonly StateWashington Washington = StateWashington.Instance;
        public static readonly StateWestVirginia WestVirginia = StateWestVirginia.Instance;
        public static readonly StateWisconsin Wisconsin = StateWisconsin.Instance;
        public static readonly StateWyoming Wyoming = StateWyoming.Instance;

        public static readonly List<State> All;
        public static readonly ReadOnlyDictionary<int, State> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static State()
        {
            All = new List<State> { Alabama, Alaska, Arizona, Arkansas, California, Colorado, Connecticut, Delaware, Florida, Georgia, Hawaii, Idaho, Illinois, Indiana, Iowa, Kansas, Kentucky, Louisiana, Maine, Maryland, Massachusetts, Michigan, Minnesota, Mississippi, Missouri, Montana, Nebraska, Nevada, NewHampshire, NewJersey, NewMexico, NewYork, NorthCarolina, NorthDakota, Ohio, Oklahoma, Oregon, Pennsylvania, RhodeIsland, SouthCarolina, SouthDakota, Tennessee, Texas, Utah, Vermont, Virginia, Washington, WestVirginia, Wisconsin, Wyoming };
            AllLookupDictionary = new ReadOnlyDictionary<int, State>(All.ToDictionary(x => x.StateID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected State(int stateID, string stateName, string statePostalCode)
        {
            StateID = stateID;
            StateName = stateName;
            StatePostalCode = statePostalCode;
        }

        [Key]
        public int StateID { get; private set; }
        public string StateName { get; private set; }
        public string StatePostalCode { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return StateID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(State other)
        {
            if (other == null)
            {
                return false;
            }
            return other.StateID == StateID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as State);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return StateID;
        }

        public static bool operator ==(State left, State right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(State left, State right)
        {
            return !Equals(left, right);
        }

        public StateEnum ToEnum => (StateEnum)GetHashCode();

        public static State ToType(int enumValue)
        {
            return ToType((StateEnum)enumValue);
        }

        public static State ToType(StateEnum enumValue)
        {
            switch (enumValue)
            {
                case StateEnum.Alabama:
                    return Alabama;
                case StateEnum.Alaska:
                    return Alaska;
                case StateEnum.Arizona:
                    return Arizona;
                case StateEnum.Arkansas:
                    return Arkansas;
                case StateEnum.California:
                    return California;
                case StateEnum.Colorado:
                    return Colorado;
                case StateEnum.Connecticut:
                    return Connecticut;
                case StateEnum.Delaware:
                    return Delaware;
                case StateEnum.Florida:
                    return Florida;
                case StateEnum.Georgia:
                    return Georgia;
                case StateEnum.Hawaii:
                    return Hawaii;
                case StateEnum.Idaho:
                    return Idaho;
                case StateEnum.Illinois:
                    return Illinois;
                case StateEnum.Indiana:
                    return Indiana;
                case StateEnum.Iowa:
                    return Iowa;
                case StateEnum.Kansas:
                    return Kansas;
                case StateEnum.Kentucky:
                    return Kentucky;
                case StateEnum.Louisiana:
                    return Louisiana;
                case StateEnum.Maine:
                    return Maine;
                case StateEnum.Maryland:
                    return Maryland;
                case StateEnum.Massachusetts:
                    return Massachusetts;
                case StateEnum.Michigan:
                    return Michigan;
                case StateEnum.Minnesota:
                    return Minnesota;
                case StateEnum.Mississippi:
                    return Mississippi;
                case StateEnum.Missouri:
                    return Missouri;
                case StateEnum.Montana:
                    return Montana;
                case StateEnum.Nebraska:
                    return Nebraska;
                case StateEnum.Nevada:
                    return Nevada;
                case StateEnum.NewHampshire:
                    return NewHampshire;
                case StateEnum.NewJersey:
                    return NewJersey;
                case StateEnum.NewMexico:
                    return NewMexico;
                case StateEnum.NewYork:
                    return NewYork;
                case StateEnum.NorthCarolina:
                    return NorthCarolina;
                case StateEnum.NorthDakota:
                    return NorthDakota;
                case StateEnum.Ohio:
                    return Ohio;
                case StateEnum.Oklahoma:
                    return Oklahoma;
                case StateEnum.Oregon:
                    return Oregon;
                case StateEnum.Pennsylvania:
                    return Pennsylvania;
                case StateEnum.RhodeIsland:
                    return RhodeIsland;
                case StateEnum.SouthCarolina:
                    return SouthCarolina;
                case StateEnum.SouthDakota:
                    return SouthDakota;
                case StateEnum.Tennessee:
                    return Tennessee;
                case StateEnum.Texas:
                    return Texas;
                case StateEnum.Utah:
                    return Utah;
                case StateEnum.Vermont:
                    return Vermont;
                case StateEnum.Virginia:
                    return Virginia;
                case StateEnum.Washington:
                    return Washington;
                case StateEnum.WestVirginia:
                    return WestVirginia;
                case StateEnum.Wisconsin:
                    return Wisconsin;
                case StateEnum.Wyoming:
                    return Wyoming;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum StateEnum
    {
        Alabama = 1,
        Alaska = 2,
        Arizona = 3,
        Arkansas = 4,
        California = 5,
        Colorado = 6,
        Connecticut = 7,
        Delaware = 8,
        Florida = 9,
        Georgia = 10,
        Hawaii = 11,
        Idaho = 12,
        Illinois = 13,
        Indiana = 14,
        Iowa = 15,
        Kansas = 16,
        Kentucky = 17,
        Louisiana = 18,
        Maine = 19,
        Maryland = 20,
        Massachusetts = 21,
        Michigan = 22,
        Minnesota = 23,
        Mississippi = 24,
        Missouri = 25,
        Montana = 26,
        Nebraska = 27,
        Nevada = 28,
        NewHampshire = 29,
        NewJersey = 30,
        NewMexico = 31,
        NewYork = 32,
        NorthCarolina = 33,
        NorthDakota = 34,
        Ohio = 35,
        Oklahoma = 36,
        Oregon = 37,
        Pennsylvania = 38,
        RhodeIsland = 39,
        SouthCarolina = 40,
        SouthDakota = 41,
        Tennessee = 42,
        Texas = 43,
        Utah = 44,
        Vermont = 45,
        Virginia = 46,
        Washington = 47,
        WestVirginia = 48,
        Wisconsin = 49,
        Wyoming = 50
    }

    public partial class StateAlabama : State
    {
        private StateAlabama(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateAlabama Instance = new StateAlabama(1, @"Alabama", @"AL");
    }

    public partial class StateAlaska : State
    {
        private StateAlaska(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateAlaska Instance = new StateAlaska(2, @"Alaska", @"AK");
    }

    public partial class StateArizona : State
    {
        private StateArizona(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateArizona Instance = new StateArizona(3, @"Arizona", @"AZ");
    }

    public partial class StateArkansas : State
    {
        private StateArkansas(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateArkansas Instance = new StateArkansas(4, @"Arkansas", @"AR");
    }

    public partial class StateCalifornia : State
    {
        private StateCalifornia(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateCalifornia Instance = new StateCalifornia(5, @"California", @"CA");
    }

    public partial class StateColorado : State
    {
        private StateColorado(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateColorado Instance = new StateColorado(6, @"Colorado", @"CO");
    }

    public partial class StateConnecticut : State
    {
        private StateConnecticut(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateConnecticut Instance = new StateConnecticut(7, @"Connecticut", @"CT");
    }

    public partial class StateDelaware : State
    {
        private StateDelaware(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateDelaware Instance = new StateDelaware(8, @"Delaware", @"DE");
    }

    public partial class StateFlorida : State
    {
        private StateFlorida(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateFlorida Instance = new StateFlorida(9, @"Florida", @"FL");
    }

    public partial class StateGeorgia : State
    {
        private StateGeorgia(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateGeorgia Instance = new StateGeorgia(10, @"Georgia", @"GA");
    }

    public partial class StateHawaii : State
    {
        private StateHawaii(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateHawaii Instance = new StateHawaii(11, @"Hawaii", @"HI");
    }

    public partial class StateIdaho : State
    {
        private StateIdaho(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateIdaho Instance = new StateIdaho(12, @"Idaho", @"ID");
    }

    public partial class StateIllinois : State
    {
        private StateIllinois(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateIllinois Instance = new StateIllinois(13, @"Illinois", @"IL");
    }

    public partial class StateIndiana : State
    {
        private StateIndiana(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateIndiana Instance = new StateIndiana(14, @"Indiana", @"IN");
    }

    public partial class StateIowa : State
    {
        private StateIowa(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateIowa Instance = new StateIowa(15, @"Iowa", @"IA");
    }

    public partial class StateKansas : State
    {
        private StateKansas(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateKansas Instance = new StateKansas(16, @"Kansas", @"KS");
    }

    public partial class StateKentucky : State
    {
        private StateKentucky(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateKentucky Instance = new StateKentucky(17, @"Kentucky", @"KY");
    }

    public partial class StateLouisiana : State
    {
        private StateLouisiana(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateLouisiana Instance = new StateLouisiana(18, @"Louisiana", @"LA");
    }

    public partial class StateMaine : State
    {
        private StateMaine(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMaine Instance = new StateMaine(19, @"Maine", @"ME");
    }

    public partial class StateMaryland : State
    {
        private StateMaryland(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMaryland Instance = new StateMaryland(20, @"Maryland", @"MD");
    }

    public partial class StateMassachusetts : State
    {
        private StateMassachusetts(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMassachusetts Instance = new StateMassachusetts(21, @"Massachusetts", @"MA");
    }

    public partial class StateMichigan : State
    {
        private StateMichigan(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMichigan Instance = new StateMichigan(22, @"Michigan", @"MI");
    }

    public partial class StateMinnesota : State
    {
        private StateMinnesota(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMinnesota Instance = new StateMinnesota(23, @"Minnesota", @"MN");
    }

    public partial class StateMississippi : State
    {
        private StateMississippi(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMississippi Instance = new StateMississippi(24, @"Mississippi", @"MS");
    }

    public partial class StateMissouri : State
    {
        private StateMissouri(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMissouri Instance = new StateMissouri(25, @"Missouri", @"MO");
    }

    public partial class StateMontana : State
    {
        private StateMontana(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateMontana Instance = new StateMontana(26, @"Montana", @"MT");
    }

    public partial class StateNebraska : State
    {
        private StateNebraska(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNebraska Instance = new StateNebraska(27, @"Nebraska", @"NE");
    }

    public partial class StateNevada : State
    {
        private StateNevada(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNevada Instance = new StateNevada(28, @"Nevada", @"NV");
    }

    public partial class StateNewHampshire : State
    {
        private StateNewHampshire(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNewHampshire Instance = new StateNewHampshire(29, @"New Hampshire", @"NH");
    }

    public partial class StateNewJersey : State
    {
        private StateNewJersey(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNewJersey Instance = new StateNewJersey(30, @"New Jersey", @"NJ");
    }

    public partial class StateNewMexico : State
    {
        private StateNewMexico(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNewMexico Instance = new StateNewMexico(31, @"New Mexico", @"NM");
    }

    public partial class StateNewYork : State
    {
        private StateNewYork(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNewYork Instance = new StateNewYork(32, @"New York", @"NY");
    }

    public partial class StateNorthCarolina : State
    {
        private StateNorthCarolina(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNorthCarolina Instance = new StateNorthCarolina(33, @"North Carolina", @"NC");
    }

    public partial class StateNorthDakota : State
    {
        private StateNorthDakota(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateNorthDakota Instance = new StateNorthDakota(34, @"North Dakota", @"ND");
    }

    public partial class StateOhio : State
    {
        private StateOhio(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateOhio Instance = new StateOhio(35, @"Ohio", @"OH");
    }

    public partial class StateOklahoma : State
    {
        private StateOklahoma(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateOklahoma Instance = new StateOklahoma(36, @"Oklahoma", @"OK");
    }

    public partial class StateOregon : State
    {
        private StateOregon(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateOregon Instance = new StateOregon(37, @"Oregon", @"OR");
    }

    public partial class StatePennsylvania : State
    {
        private StatePennsylvania(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StatePennsylvania Instance = new StatePennsylvania(38, @"Pennsylvania", @"PA");
    }

    public partial class StateRhodeIsland : State
    {
        private StateRhodeIsland(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateRhodeIsland Instance = new StateRhodeIsland(39, @"Rhode Island", @"RI");
    }

    public partial class StateSouthCarolina : State
    {
        private StateSouthCarolina(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateSouthCarolina Instance = new StateSouthCarolina(40, @"South Carolina", @"SC");
    }

    public partial class StateSouthDakota : State
    {
        private StateSouthDakota(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateSouthDakota Instance = new StateSouthDakota(41, @"South Dakota", @"SD");
    }

    public partial class StateTennessee : State
    {
        private StateTennessee(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateTennessee Instance = new StateTennessee(42, @"Tennessee", @"TN");
    }

    public partial class StateTexas : State
    {
        private StateTexas(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateTexas Instance = new StateTexas(43, @"Texas", @"TX");
    }

    public partial class StateUtah : State
    {
        private StateUtah(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateUtah Instance = new StateUtah(44, @"Utah", @"UT");
    }

    public partial class StateVermont : State
    {
        private StateVermont(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateVermont Instance = new StateVermont(45, @"Vermont", @"VT");
    }

    public partial class StateVirginia : State
    {
        private StateVirginia(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateVirginia Instance = new StateVirginia(46, @"Virginia", @"VA");
    }

    public partial class StateWashington : State
    {
        private StateWashington(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateWashington Instance = new StateWashington(47, @"Washington", @"WA");
    }

    public partial class StateWestVirginia : State
    {
        private StateWestVirginia(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateWestVirginia Instance = new StateWestVirginia(48, @"West Virginia", @"WV");
    }

    public partial class StateWisconsin : State
    {
        private StateWisconsin(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateWisconsin Instance = new StateWisconsin(49, @"Wisconsin", @"WI");
    }

    public partial class StateWyoming : State
    {
        private StateWyoming(int stateID, string stateName, string statePostalCode) : base(stateID, stateName, statePostalCode) {}
        public static readonly StateWyoming Instance = new StateWyoming(50, @"Wyoming", @"WY");
    }
}