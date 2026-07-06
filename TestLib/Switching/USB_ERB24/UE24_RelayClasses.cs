using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ABT.TestSpace.TestExec.Switching.RelayForms;
using static ABT.TestSpace.TestExec.Switching.USB_ERB24.UE24;

namespace ABT.TestSpace.TestExec.Switching.USB_ERB24 {
    public enum SWITCHED_STATE { disconnected, CONNECTED };

    public sealed class SwitchedNet {
        public readonly String ID;
        public readonly String Alias;

        public SwitchedNet(String ID, String Alias) { this.ID = ID; this.Alias = Alias; }

        public override Boolean Equals(Object obj) {
            SwitchedNet sn = obj as SwitchedNet;
            if (ReferenceEquals(this, sn)) return true;
            return sn != null && sn.ID == ID && sn.Alias == Alias;
        }

        public override Int32 GetHashCode() { return 3 * ID.GetHashCode() + Alias.GetHashCode(); }

        public override String ToString() { return ID; }
    }

    public sealed class Relay {
        public readonly UE UE;
        public readonly R R;
        public readonly SwitchedNet C;
        public readonly SwitchedNet NC;
        public readonly SwitchedNet NO;

        public Relay(UE UE, R R, SwitchedNet C, SwitchedNet NC, SwitchedNet NO) {
            this.UE = UE; this.R = R; this.C = C; this.NC = NC; this.NO = NO;
            Validate();
        }

        private void Validate() {
            if (String.Equals(C.Alias, String.Empty)) throw new ArgumentException($"Relay terminal Common '{C.Alias}' cannot be String.Empty.");
            if (C == NO) throw new ArgumentException($"Relay terminals Common '{C}' & Normally Open '{NO}' cannot be identical.");
            if (C == NC) throw new ArgumentException($"Relay terminals Common '{C}' & Normally Closed '{NC}' cannot be identical.");
            if (NC == NO) throw new ArgumentException($"Relay terminals Normally Closed '{NC}' & Normally Open '{NO}' cannot be identical.");
        }

        public static String GetUE(UE ue) { return Enum.GetName(typeof(UE), ue); }
        public static String GetR(R r) { return Enum.GetName(typeof(R), r); }

        public C.S Get() { return UE24.Get(UE, R); }

        public void Set(C.S state) { UE24.Set(UE, R, state); }

        public Boolean Is(C.S state) { return UE24.Is(UE, R, state); }

        public override Boolean Equals(Object obj) {
            Relay r = obj as Relay;
            if (ReferenceEquals(this, r)) return true;
            return r != null && r.UE == UE && r.R == R && C == r.C && NC == r.NC && NO == r.NO;
        }

        public override Int32 GetHashCode() { return 3 * UE.GetHashCode() + R.GetHashCode(); }
    }

    public sealed class Terminal {
        public readonly UE UE;
        public readonly R R;
        public readonly C.T T;

        public Terminal(UE UE, R R, C.T T) { this.UE = UE; this.R = R; this.T = T; }

        public override Boolean Equals(Object obj) {
            Terminal t = obj as Terminal;
            if (ReferenceEquals(this, t)) return true;
            return t != null && t.UE == UE && t.R == R && t.T == T;
        }

        public override Int32 GetHashCode() { return 3 * UE.GetHashCode() + R.GetHashCode() + T.GetHashCode(); }
    }

    public sealed class State {
        public readonly UE UE;
        public readonly R R;
        public readonly C.S S;

        public State(UE UE, R R, C.S S) { this.UE = UE; this.R = R; this.S = S; }

        public override Boolean Equals(Object obj) {
            State s = obj as State;
            if (ReferenceEquals(this, s)) return true;
            return s != null && s.UE == UE && s.R == R && s.S == S;
        }

        public override Int32 GetHashCode() { return 3 * UE.GetHashCode() + R.GetHashCode() + S.GetHashCode(); }
    }

    public sealed class SwitchedRoute {
        public readonly Tuple<SwitchedNet, SwitchedNet> SwitchedNetPair;
        public SwitchedRoute(Tuple<SwitchedNet, SwitchedNet> switchedNetPair) { SwitchedNetPair = switchedNetPair; }

        public Boolean Contains(SwitchedNet SN) { return (SwitchedNetPair.Item1 == SN) || (SwitchedNetPair.Item2 == SN); }

        public override Boolean Equals(Object obj) {
            if (!(obj is SwitchedRoute sr)) return false;
            if (ReferenceEquals(this, sr)) return true;
            if (sr.SwitchedNetPair.Item1 == SwitchedNetPair.Item1 && sr.SwitchedNetPair.Item2 == SwitchedNetPair.Item2) return true;
            if (sr.SwitchedNetPair.Item1 == SwitchedNetPair.Item2 && sr.SwitchedNetPair.Item2 == SwitchedNetPair.Item1) return true;
            return false;
        }

        public override Int32 GetHashCode() { return 3 * SwitchedNetPair.GetHashCode(); }
    }

    public sealed class SwitchedRoutes {
        // TODO:  Eventually; optimize Are & Set to invoke Are(UE ue, Dictionary<R, C.S> RεS) & Set(UE ue, Dictionary<R, C.S> RεS) for optimally simultaneous switching.
        public readonly Dictionary<SwitchedRoute, HashSet<State>> SRs;

        public SwitchedRoutes(Dictionary<SwitchedRoute, HashSet<State>> RouteStates) { SRs = RouteStates; }

        public Boolean Are(SwitchedNet SN1, SwitchedNet SN2, SWITCHED_STATE SwitchedState) {
            Boolean are = true;
            foreach (State s in SRs[SwitchedRouteGet(SN1, SN2)]) are &= Is(s.UE, s.R, s.S); 
            switch (SwitchedState) {
                case SWITCHED_STATE.disconnected:  return !are;
                case SWITCHED_STATE.CONNECTED:     return are;
                default:                           throw new NotImplementedException();
            }
        }

        public Boolean Are(SwitchedNet SN, HashSet<SwitchedNet> SNs, SWITCHED_STATE SwitchedState) {
            Boolean are = true;
            foreach (SwitchedNet sn in SNs) are &= Are(SN, sn, SwitchedState);
            return are;
        }

        public Boolean Connectable(SwitchedNet SN1, SwitchedNet SN2) { return SRs.ContainsKey(new SwitchedRoute(Tuple.Create(SN1, SN2))); }

        public Boolean Connectable(SwitchedNet SN, HashSet<SwitchedNet> SNs) {
            Boolean connectable = true;
            foreach (SwitchedNet sn in SNs) connectable &= Connectable(SN, sn);
            return connectable;
        }

        public HashSet<SwitchedRoute> RoutesGet(SwitchedNet SN) {
            HashSet<SwitchedRoute> switchedRoutes = new HashSet<SwitchedRoute>();
            foreach (KeyValuePair<SwitchedRoute, HashSet<State>> kvp in SRs) { if (kvp.Key.Contains(SN)) switchedRoutes.Add(kvp.Key); }
            return switchedRoutes;
            // If can convert:
            //      - Dictionary<String, HashSet<Terminal>>
            // to/from:
            //      - Dictionary<UE, Dictionary<R, C.S>>
            // Can invoke:
            //       - Set(Dictionary<UE, Dictionary<R, C.S>> UEεRεS)
            //       - Are(Dictionary<UE, Dictionary<R, C.SC>> UEεRεS)
            //       - Get()
            //
            // Initially support ERB24's Set(), Is() & Get() functions for discrete/single (UE, R, C.S)
        }

        private static C.S StateNegate(C.S State) { return (State == C.S.NO) ? C.S.NC : C.S.NO; }

        public void Set(SwitchedNet SN1, SwitchedNet SN2, SWITCHED_STATE SwitchedState) {
            switch (SwitchedState) {
                case SWITCHED_STATE.disconnected:  foreach (State s in SRs[SwitchedRouteGet(SN1, SN2)]) UE24.Set(s.UE, s.R, StateNegate(s.S));  break;
                case SWITCHED_STATE.CONNECTED:     foreach (State s in SRs[SwitchedRouteGet(SN1, SN2)]) UE24.Set(s.UE, s.R, s.S);               break;
                default:                           throw new NotImplementedException();
            }
        }

        public void Set(SwitchedNet SN, HashSet<SwitchedNet> SNs, SWITCHED_STATE SwitchedState) { foreach (SwitchedNet sn in SNs) Set(SN, sn, SwitchedState); }

        public void Switch(SwitchedNet SN, SwitchedNet From, SwitchedNet To) {
            Set(SN, From, SWITCHED_STATE.disconnected);
            Set(SN, To, SWITCHED_STATE.CONNECTED);
        }

        private SwitchedRoute SwitchedRouteGet(SwitchedNet SN1, SwitchedNet SN2) {
            SwitchedRoute sr = new SwitchedRoute(Tuple.Create(SN1, SN2));
            if (!SRs.ContainsKey(sr)) sr = new SwitchedRoute(Tuple.Create(SN2, SN1)); // If at first you don't succeed...
            if (!SRs.ContainsKey(sr)) throw new ArgumentException($"Invalid Route SwitchedNets SN1 '{SN1}' & SN2 '{SN2}'.");
            return sr;
        }

    }

    public sealed class Relays {
        public readonly HashSet<Relay> Rs;
        public readonly Dictionary<String, HashSet<Terminal>> SNTs = new Dictionary<String, HashSet<Terminal>>();

        public Relays(HashSet<Relay> rs) {
            Rs = rs;
            Validate();
        }

        private void Validate() {
            StringBuilder sb = new StringBuilder($"Cannot currently accomodate USB-ERB24 Relays connected serially:{Environment.NewLine}Boards/Relays");
            List<(Relay, Relay)> rs =
                (from r1 in Rs
                 from r2 in Rs
                 where (r1.C == r2.NC || r1.C == r2.NO)
                 select (r1, r2)).ToList();
            if (rs.Count() != 0) {
                foreach ((Relay r1, Relay r2) in rs) {
                    sb.AppendLine("Below relay pair {R1, R2} serially connected, C1 to (NC2 ⨁ NO2)");
                    sb.AppendLine($"   B1='{Relay.GetUE(r1.UE)}', R1='{Relay.GetR(r1.R)}', C1='{r1.C}', NC1='{r1.NC}', NO1='{r1.NO}'.");
                    sb.AppendLine($"   B2='{Relay.GetUE(r2.UE)}', R2='{Relay.GetR(r2.R)}', C2='{r2.C}', NC2='{r2.NC}', NO2='{r2.NO}'.");
                    sb.AppendLine("");
                }
                throw new InvalidOperationException(sb.ToString());
            }
        }
    }
}
