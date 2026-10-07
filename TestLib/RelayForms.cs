namespace ABT.TestSpace.TestExec.Switching {
        /// <summary>Relay Forms A, B &amp; C.</summary>
    public static class RelayForms {
        /// <summary>Relay Form A.</summary>
        public static class A {
            /// <summary>Form A States.</summary>
            public enum S {
                /// <summary>S.NO; Form A relay is de-energized and in normally opened state.</summary>
                NO,
                /// <summary>S.C;  Form A relay is energized and in abnormally closed state.</summary>
                C
            }

            /// <summary>Form A Terminals.</summary>
            public enum T {
                /// <summary>T.C; Form A relay Common Terminal.</summary>
                C,
                /// <summary>T.NO; Form A relay Normally Open Terminal.</summary>
                NO
            }
        }

        /// <summary>Relay Form B.</summary>
        public static class B {
            /// <summary>Form B States.</summary>
            public enum S {
                /// <summary>B.NC; Form B relay is de-energized and in normally closed state.</summary>
                NC,
                /// <summary>B.O;  Form B relay is energized and in abnormally opened state.</summary>
                O
            }

            /// <summary>Form B Terminals.</summary>
            public enum T {
                /// <summary>T.C; Form B relay Common Terminal.</summary>
                C,
                /// <summary>T.NC; Form B relay Normaly Closed Terminal.</summary>
                NC
            }
        }

        /// <summary>Relay Form C.</summary>
        public static class C {
            /// <summary>Form C States.</summary>
            public enum S {
                /// <summary>S.NC; Form C relay is de-energized and in normally closed/normally open state, with common C terminal connected to NC terminal &amp; disconnected from NO terminal.</summary>
                NC,
                /// <summary>S.NO; Form C relay is energized and in abnormally opened/abnormally closed state, with common C terminal disconnected from NC terminal &amp; connected to NO terminal.</summary>
                NO
            }

            /// <summary>Form C Terminals.</summary>
            public enum T {
                /// <summary>T.C; Form C relay Common Terminal.</summary>
                C,
                /// <summary>T.NO; Form C relay Normally Closed Terminal.</summary>
                NC,
                /// <summary>T.NO; Form C relay Normally Open Terminal.</summary>
                NO
            }
        }
    }
}
