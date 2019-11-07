using System.Collections.Generic;

namespace lib
{
    public class Slot{
        public string location;
        public string date;

        public Slot(string location, string date){
            this.location = location;
            this.date = date;
        }

    }

    public class MeetingProposal{
        public string coordinator;
        public string topic;
        public int minParticipants;
        public List<Slot> slotList;
        public List<string> invitees; //can be empty

        public MeetingProposal(string coordinator, string topic, int minParticipants,
                        List<Slot> slotList , List<string> invitees){
            this.coordinator = coordinator;
            this.topic = topic;
            this.minParticipants = minParticipants;
            this.slotList = slotList;
            this.invitees = invitees;
        }
    }
}