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
        string coordinator;
        string topic;
        int minParticipants;
        List<Slot> slotList;
        List<string> invitees; //can be null

        MeetingProposal(string coordinator, string topic, int minParticipants,
                        List<Slot> slotList){
            MeetingProposal(coordinator, topic, minParticipants, slotList, null);
        }
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