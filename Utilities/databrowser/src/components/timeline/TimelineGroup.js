import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import TimelineTick from './TimelineTick.js'
import Grid from '@material-ui/core/Grid';
import moment from 'moment';


function sortEvents(a,b) {
  if (moment(a.on).isBefore(moment(b.on)))
    return 1;
  if (moment(b.on).isBefore(moment(a.on)))
    return -1;
  return 0;
}


class TimelineGroup extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {

    const { classes } = this.props;

    if(this.props.group.events.length == 0) return (<div></div>);

    var events = this.props.group.events.sort(sortEvents);
    var end = moment(events[events.length -1].on).startOf('hour');
    var start = moment(events[0].on).endOf('hour').add(1,'m');
    var current = moment(events[0].on).endOf('hour').add(1,'m');

    var ticks = [];
    var eventIndex = 0;


    while(current > end)
    {

       // Initialize new tick
       var tick = {
         description: current.format('h:mm a'),
         id: current.valueOf(),
         events: []
       };

       // Build the next tick time
       const next = current.clone().subtract(10, 'minutes');

       // Cycle through th enext events and add (assumes events are sorted by time desc)
       var loopEvents = true;

       while(loopEvents == true && eventIndex < events.length)
       {
         var nextEventTime = moment(events[eventIndex].on);

         if(current.isSameOrAfter(nextEventTime) && next.isBefore(nextEventTime))
         {
            tick.events.push(events[eventIndex]);
            eventIndex ++;
         }
         else {
           loopEvents = false;
         }
       }

       // Add and move on
       ticks.push(tick);

       current = next;
    }


    return (<div>
      {ticks.map(n => {
        return (<TimelineTick
          viewState={this.props.viewState}
          events={n.events}
          description={n.description} />
        );
      })}
      </div>
    );
  }
}



export default  (TimelineGroup);
