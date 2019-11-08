using System.Collections.Generic;
using System;

namespace lib{
    
    [Serializable]
    public class Location{
        public string location_name;
        public List<Room> roomList;

        public Location(string location_name){
            this.location_name = location_name;
        }

        public Room addRoom(string room_name, int capacity){
            Room room = new Room(this, capacity, room_name);
            roomList.Add(room);
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
        }
    }

    [Serializable]
    public class MeetingProposal{
        public string coordinator;
        public string topic;
        public int minParticipants;
        public List<Slot> slotList;
        public List<string> invitees; //can be empty
        public int numParticipants;
        public bool open;
        public Room room;
        public string date;

        public MeetingProposal(string coordinator, string topic, int minParticipants,
                        List<Slot> slotList , List<string> invitees){
            this.coordinator = coordinator;
            this.topic = topic;
            this.minParticipants = minParticipants;
            this.slotList = slotList;
            this.invitees = invitees;
            this.numParticipants = 0;
            this.open = true;
        }

        public override string ToString(){

            string text = "";
            text += "Coordinator: " + coordinator + "\n";
            text += "Topic: " + topic + "\n";
            text += "Minimum Participants: " + minParticipants.ToString() + "\n";
            
            foreach(Slot slot in slotList){
                text += slot.location + ':' + slot.date + "\n";
            }
            foreach(string invitee in invitees){
                text += invitee + "\n";
            }

            return text;
        }

        public void close(Room room, string date){
            this.open = false;
            this.room = room;
            this.date = date;
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
    }
}