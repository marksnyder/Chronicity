import Moment from 'moment';
import { extendMoment } from 'moment-range';

const moment = extendMoment(Moment);

function sortEvents(a,b) {
  if (moment(a.on).isBefore(moment(b.on)))
    return -1;
  if (moment(a.on).isAfter(moment(b.on)))
    return 1;
  return 0;
}

class DataUtilities {

  static mergeStateChanges = (all,changes, groupKey, color) => {

    if(!all.hasOwnProperty(groupKey)) all[groupKey] = [];

    changes.forEach(function(e){
      e.color = color
    });

    var group =  all[groupKey].slice();
    var merged = group.concat(changes);
    all[groupKey] = merged;

    return all;

  };

  static mergeEvents = (all,changes,iconStyle,initials) => {

    changes.forEach(function(e){
      e.iconStyle = iconStyle;
      e.initials = initials;
    })

    return all.concat(changes);

  };


  static findApplicableStateChanges(start,end,changes)
  {
    var result = [];
    changes.forEach(function(c) {
      var range1 = moment.range(start, end);
      var range2 = moment.range(c.start, c.end);
      if(range1.overlaps(range2)) result.push(c);
    });
    return result;
  }

  static findApplicableEvents(start,end,events)
  {
    var result = [];
    events.forEach(function(e) {
      if(moment(e.on).isBetween(start,end,null,'[]')) {
        result.push(e);
      }
    });
    return result;
  }


  static groupByDay = (events,stateChanges) => {
    events = events.sort(sortEvents);
    var start = moment(events[0].on).startOf('day');
    var end = moment(events[events.length -1].on).endOf('day');
    return DataUtilities.groupBy(events,stateChanges,start,end,'MMMM Do YYYY','days',1, false).reverse();
  }

  static groupByHour = (events,stateChanges,start,end) => {
    events = events.sort(sortEvents);
    return DataUtilities.groupBy(events,stateChanges,start,end,'h:mm a','hours',1, true).reverse();
  }

  static groupBy10Minutes = (events,stateChanges,start,end) => {
    events = events.sort(sortEvents);
    return DataUtilities.groupBy(events,stateChanges,start,end,'h:mm a','minutes',10, true).reverse();
  }

  static groupBy = (events, stateChanges, start, end, descFormat, incrementType, increment, emptyGroups) => {

    var groups = [];
    var current = moment(moment(start).format());

    while(current.isSameOrBefore(end))
    {
       var endOfGroup = current.clone().add(increment, incrementType).subtract(1,'seconds');

       var group = {
         description:  current.format(descFormat),
         id: current.valueOf() + incrementType,
         events: [],
         stateChanges: {},
         start: current.format(),
         end: endOfGroup.format()
       };

       var e = DataUtilities.findApplicableEvents(current,endOfGroup,events);
       if(e.length > 0)  {
         group.events = group.events.concat(e);
       }


       var stateKeys = Object.keys(stateChanges);

       stateKeys.forEach(function(k) {
            group.stateChanges[k] = DataUtilities.findApplicableStateChanges(current,endOfGroup,stateChanges[k])
       });

       if(group.events.length > 0 || emptyGroups) groups.push(group);
       current.add(increment, incrementType);
    }

    return groups;
  }

}

export default (DataUtilities);
