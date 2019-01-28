using System;

namespace RoboticsTools {
    public class Data {
        public IDataObject service;

        public Data(IDataObject service) {
            this.service = service;
        }
    }
}