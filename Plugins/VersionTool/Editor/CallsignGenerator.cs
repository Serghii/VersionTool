using UnityEngine;

namespace Plugins.VersionTool
{
    public static class CallsignGenerator
    {
        private static readonly string[] _callsignFirst = new string[]
        {
            "Avaricious", "Benign", "Crumbly", "Despairing", "Envious", "Fine", "Grumbling", "Honeyed", "Igneous", "Juvenile", "Kinetic", "Lovely", "Mumbling", "Neon", "Overt", "Pernicious", "Quizzical", "Ravenous", "Surprise", "Tempting", "Unexpected", "Vacuous", "Wizened", "Yelling", "Boring", "Rigid", "Angry", "Detached", "Tapering", "Fragrant", "Distracted", "Proud", "Temporary", "Important", "Dreadful", "Benevolent", "Vacant", "Hopeful", "Frugal", "Moist", "Timid", "Irritating", "Flawless", "Exquisite", "Perfect", "Unfettered", "Rampant", "Full", "Baked", "Wonderful", "Yesterday's", "Dangerous", "Frisky", "Playful", "Squishy", "Podgy", "Rough", "Tiny", "Wobbly", "Bearded", "Wasted", "Fulsome", "Devastating", "Stupendous", "Sizeable", "Upended", "Joyful", "Impatient", "Robotic", "Misplaced", "Perky", "Derivative", "Irksome", "Rampaging", "Afternoon",
        };

        private static readonly string[] _callsignSecond = new string[]
        {
            "Aphid", "Bumblebee", "Caterpillar", "Damselfly", "Earwig", "Flatworm", "Grub", "Hornet", "Ladybird", "Millipede", "Owl", "Silverfish", "Tardigrade", "Wasp", "Herring", "Pancake", "Hitchhiker", "Badger", "Yeti", "Trampoline", "Pinecone", "Ghost", "Whelk", "Potato", "Breadbin", "Skeleton", "Omelette", "Teapot", "Soup", "Tentacle", "Crab", "Beekeeper", "Mule", "Kebab", "Caravan", "Cornflake", "Truck", "Pipes", "Otter", "Possum", "Dinnertime", "Breakfast", "Leg", "Tip", "Crust", "Inpatient", "Vestibule", "Pudding", "Fanatic", "Drip", "Portion", "Bathroom", "Watermelon", "Cube", "Anticeptic",
        };

        public static string GenerateStupidCallsign(string _version)
        {
            int hash = Animator.StringToHash(_version); // crc32
            int firstIndex = (hash & 0xffff) % _callsignFirst.Length;
            int secondIndex = ((hash >> 16) & 0xffff) % _callsignSecond.Length;
            return $"{_callsignFirst[firstIndex]} {_callsignSecond[secondIndex]}";
        }
    }
}