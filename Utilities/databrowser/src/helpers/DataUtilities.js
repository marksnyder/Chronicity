import Moment from 'moment';
import { extendMoment } from 'moment-range';

const moment = extendMoment(Moment);

function sortMarkers(a,b) {
  if (moment(a.on).isBefore(moment(b.on)))
    return -1;
  if (moment(a.on).isAfter(moment(b.on)))
    return 1;
  return 0;
}

class DataUtilities {

  static mergeTrackerChanges = (all,changes, groupKey, color) => {

    if(!all.hasOwnProperty(groupKey)) all[groupKey] = [];

    changes.forEach(function(e){
      e.color = color;
    });

    var group =  all[groupKey].slice();
    var merged = group.concat(changes);
    all[groupKey] = merged;

    return all;

  };

  static mergeMarkers = (all,markers,callback) => {

    var newList = [];
    markers.forEach(function(e){
      var data = callback(e);
      
      newList.push(data);
    });

    return all.concat(newList);
  };


  static findApplicableTrackerChanges(start,end,changes)
  {
    var result = [];
    changes.forEach(function(c) {
      var range1 = moment.range(start, end);
      var range2 = moment.range(c.start, c.end);
      if(range1.overlaps(range2)) result.push(c);
    });
    return result;
  }

  static findApplicableMarkers(start,end,markers)
  {
    var result = [];
    markers.forEach(function(e) {
      if(moment(e.on).isBetween(start,end,null,'[]')) {
        result.push(e);
      }
    });
    return result;
  }


  static groupByDay = (markers,trackerChanges) => {
    markers = markers.sort(sortMarkers);
    var start = moment(markers[0].on).startOf('day');
    var end = moment(markers[markers.length -1].on).endOf('day');
    return DataUtilities.groupBy(markers,trackerChanges,start,end,'MMMM Do YYYY','days',1, false).reverse();
  }

  static groupByHour = (markers,trackerChanges,start,end) => {
    markers = markers.sort(sortMarkers);
    return DataUtilities.groupBy(markers,trackerChanges,start,end,'h:mm a','hours',1, true).reverse();
  }

  static groupBy10Minutes = (markers,trackerChanges,start,end) => {
    markers = markers.sort(sortMarkers);
    return DataUtilities.groupBy(markers,trackerChanges,start,end,'h:mm a','minutes',10, true).reverse();
  }

  static groupBy = (markers, trackerChanges, start, end, descFormat, incrementType, increment, emptyGroups) => {

    var groups = [];
    var current = moment(moment(start).format());

    while(current.isSameOrBefore(end))
    {
       var endOfGroup = current.clone().add(increment, incrementType).subtract(1,'seconds');

       var group = {
         description:  current.format(descFormat),
         id: current.valueOf() + incrementType,
         markers: [],
         trackerChanges: {},
         start: current.format(),
         end: endOfGroup.format()
       };

       var e = DataUtilities.findApplicableMarkers(current,endOfGroup,markers);
       if(e.length > 0)  {
         group.markers = group.markers.concat(e);
       }


       var trackerKeys = Object.keys(trackerChanges);

       trackerKeys.forEach(function(k) {
            group.trackerChanges[k] = DataUtilities.findApplicableTrackerChanges(current,endOfGroup,trackerChanges[k])
       });

       if(group.markers.length > 0 || emptyGroups) groups.push(group);
       current.add(increment, incrementType);
    }

    return groups;
  }

}

export default (DataUtilities);
