using System.Collections.Generic;

namespace lib
{
    public class Slot{
        private string date;
        private string location;

        public Slot(string date, string location){
            this.date = date;
            this.location = location;
        }
        public string getDate(){
            return this.date;
        }
        public string getLocation(){
            return this.location;
        }

    }

    public class MeetingProposal{
        public string coordinator;
        public string topic;
        public int minParticipants;
        public List<Slot> slotList;
        public List<string> invitees; //can be null

        //test if this works, seems wrong
        MeetingProposal(string coordinator, string topic, int minParticipants,
                        List<Slot> slotList) : this(coordinator, topic, minParticipants, slotList, null)
        {}
        MeetingProposal(string coordinator, string topic, int minParticipants,
                        List<Slot> slotList , List<string> invitees){
            this.coordinator = coordinator;
            this.topic = topic;
            this.minParticipants = minParticipants;
            this.slotList = slotList;
            this.invitees = invitees;
        }
    }
}