using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace lib{
    
    [Serializable]
    public class Location{
        public string location_name;
        public Dictionary<string,Room> roomList;

        public Location(string location_name){
            this.location_name = location_name;
            roomList = new Dictionary<string, Room>();
        }

        public Room addRoom(string room_name, int capacity){
            Room room = new Room(this, capacity, room_name);
            roomList.Add(room.room_name, room);
            return room;
        }
    }

    [Serializable]
    public class Slot{
        public string location;
        public string date;

        public Slot(string location, string date){
            this.location = location;
            this.date = date;
        }

        // override object.Equals
        public override bool Equals(object obj){

            if (obj == null || GetType() != obj.GetType()){
                return false;
            } else if(this.location == ((Slot)obj).location &&
                    this.date == ((Slot)obj).date){
                return true;
            } else{
                return false;
            }
        }

        //overide object.GetHashCode
		public override int GetHashCode(){
            
			var hashCode = -1573050897;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(location);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(date);
			return hashCode;
		}
	}

    [Serializable]
    public class Room{
        public Location location;
        public int capacity;
        public string room_name;
        public List<string> usedDates;

        public Room(Location location, int capacity, string room_name){
            this.location = location;
            this.capacity = capacity;
            this.room_name = room_name;
            this.usedDates = new List<string>();
        }

        public Room(Room room){
            this.location = room.location;
            this.capacity = room.capacity;
            this.room_name = String.Copy(room.room_name);
            this.usedDates = new List<string>(usedDates);
        }

        public void book(string date){
            usedDates.Add(date);
        }
    }

    [Serializable]
    public class Participant{
        public ClientInfo client;
        public List<Slot> slotList;

        public Participant(ClientInfo client, List<Slot> slotList){
            this.client = client;
            this.slotList = slotList;
        }
    }

    [Serializable]
    public class MeetingProposal{
        public string coordinator;
        public string topic;
        public int minParticipants;
        public List<Slot> slotList;
        public List<string> invitees; //can be empty
        public List<Participant> participants;
        public bool open;
        public bool canceled;
        public Room room;
        public string date;
        public int version;

        public MeetingProposal(string coordinator, string topic, int minParticipants,
                        List<Slot> slotList , List<string> invitees){
            this.coordinator = coordinator;
            this.topic = topic;
            this.minParticipants = minParticipants;
            this.slotList = slotList;
            this.invitees = invitees;
            this.participants = new List<Participant>();
            this.open = true;
            this.canceled = false;
            this.version = 1;
        }

        public MeetingProposal(MeetingProposal meeting){
            this.coordinator = String.Copy(meeting.coordinator);
            this.topic = String.Copy(meeting.topic);
            this.minParticipants = meeting.minParticipants;
            this.slotList = new List<Slot>(meeting.slotList);
            this.invitees = new List<string>(meeting.invitees);
            this.participants = new List<Participant>(meeting.participants);
            this.open = meeting.open;
            if (room != null) this.room = new Room(meeting.room);
            if (date != null) this.date = String.Copy(meeting.date);
            this.version = meeting.version;
        }

        public override string ToString(){

            string text = "";
            text += "Topic: " + topic + "\n";
            text += "Version: " + version.ToString() + "\n";
            text += "Coordinator: " + coordinator + "\n";
            text += "Minimum Participants: " + minParticipants.ToString() + "\n";
            
            text += "Slots:\n";
            foreach(Slot slot in slotList){
                text += "\t" + slot.location + ':' + slot.date + "\n";
            }
            text += "Invited clients:\n";
            foreach(string invitee in invitees){
                text += "\t" + invitee + "\n";
            }
            text += "Participants\n";
            foreach(Participant participant in participants){
                text += "\t" + participant.client.username + "\n";
            }
            if (open){
                text += "This meeting is open";
            } else{
                text += "This meeting is closed";
            }

            return text;
        }

        public void join(Participant p){
            this.participants.Add(p);
            this.updateVersion();
        }

        public void close(Room room, string date){
            if(this.canceled) return;
            this.open = false;
            this.room = room;
            this.date = date;
            room.book(date);
            this.updateVersion();
        }

        public void updateVersion(){
            this.version += 1;
        }
    }

    [Serializable]
    public class MeetingException : Exception{
        public MeetingException(){
        }

        public MeetingException(string message)
            : base(message){
        }

        public MeetingException(string message, Exception inner)
            : base(message, inner){
        }

        public MeetingException(SerializationInfo info, StreamingContext context)
            : base(info, context){
        }
    }
}