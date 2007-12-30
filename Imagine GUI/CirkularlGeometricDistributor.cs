using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.GUI
{
    public class CirkularGeometricDistributor<T>
    {
        private List<Group<T>> groups = new List<Group<T>>();
        private double unitLength;
        private double circumference;

        public CirkularGeometricDistributor(double circumference, double unitLength)
        {
            this.unitLength = unitLength;
            this.circumference = circumference;
        }

        public void AddUnit(T key, double position)
        {
            position = normalize(position);
            Group<T> group = new Group<T>(circumference, unitLength);
            group.AddMember(key, position);
            Group<T> otherGroup;
            while((otherGroup = findCollision(group)) != null)
            {
                group.Absorb(otherGroup);
                groups.Remove(otherGroup);
            }
            groups.Add(group);
        }

        public Dictionary<T, double> getPositions()
        {
            Dictionary<T, double> positions = new Dictionary<T, double>();
            foreach (Group<T> group in groups)
                foreach (KeyValuePair<T, double> pair in group.AdjustedMembers)
                    positions[pair.Key] = pair.Value;
            
            return positions;
        }

        private Group<T> findCollision(Group<T> targetGroup)
        {
            foreach (Group<T> group in groups)
            {
                if (group.collidesWith(targetGroup))
                    return group;
            }

            return null;
        }

        private double normalize(double value)
        {
            return (circumference + value) % circumference;
        }
    }

    class Group<T>
    {
        private List<T> orderedKeys = new List<T>();
        private Dictionary<T, double> members = new Dictionary<T, double>();
        private double circumference;
        private double unitLength;

        public Group(double circumference, double unitLength)
        {
            this.circumference = circumference;
            this.unitLength = unitLength;
        }

        public Dictionary<T, double> Members
        {
            get { return members; }
        }

        public void AddMember(T key, double position)
        {
            members[key] = normalize(position);
            orderedKeys.Add(key);
        }

        public void Absorb(Group<T> otherGroup)
        {
            Dictionary<T, double> otherMembers = otherGroup.members;
            foreach (KeyValuePair<T, double> pair in otherMembers)
            {
                members[pair.Key] = pair.Value;
                orderedKeys.Insert(0, pair.Key);
            }
        }

        public bool collidesWith(Group<T> targetGroup)
        {
            double myCenter = CenterPosition;
            double myLength = Length;
            double targetCenter = targetGroup.CenterPosition;
            double targetLength = targetGroup.Length;
            double collLength = (targetLength + Length) / 2.0;

            if (normalize(targetCenter - myCenter) < collLength
              ||normalize(myCenter - targetCenter) < collLength)
                return true;
            else
                return false;

        }

        public double CenterPosition
        {
            get
            {
                if (members.Count == 1)
                    return members[orderedKeys[0]];

                double sum = 0.0;
                bool first = true;
                double last = 0.0;
                foreach (double position in members.Values)
                {
                    if (first)
                    {
                        sum = position;
                        first = false;
                        last = position;
                    }
                    else
                    {
                        double delta = minDelta(last, position);
                        double normPosition = last + delta;
                        sum += normPosition;
                        last = normPosition;
                    }
                }
                return normalize(sum / members.Count);
            }
        }

        public double Length
        {
            get { return members.Count * unitLength; }
        }

        public Dictionary<T, double> AdjustedMembers
        {
            get
            {
                if (members.Count == 1)
                    return new Dictionary<T, double>(members);

                double startPosition = CenterPosition - (members.Count - 1) * unitLength / 2.0;
                List<T> sortedMembers = new List<T>();
                foreach (T key in orderedKeys)
                {
                    bool inserted = false;
                    for (int i = 0; i < sortedMembers.Count; i++)
                    {
                        if (minDelta(members[key], members[sortedMembers[i]]) > 0)
                        {
                            inserted = true;
                            sortedMembers.Insert(i, key);
                            break;
                        }
                    }
                    if (!inserted)
                        sortedMembers.Add(key);
                }

                Dictionary<T, Double> retval = new Dictionary<T, double>();
                for (int i = 0; i < sortedMembers.Count; i++)
                    retval[sortedMembers[i]] = normalize(startPosition + i * unitLength);

                return retval;
            }
        }

        private double normalize(double value)
        {
            return (circumference + value) % circumference;
        }

        private double minDelta(double value1, double value2)
        {
            double delta = value2 - value1;
            double absDelta = Math.Abs(delta);
            if (absDelta <= circumference / 2.0)
                return delta; 
            else
                if(delta > 0)
                    return -circumference + delta;
                else
                    return circumference + delta;
        }
    }
}
