import React from 'react';
import PropTypes from 'prop-types';
import DialogTitle from '@material-ui/core/DialogTitle';
import moment from 'moment';
import Chip from '@material-ui/core/Chip';
import Typography from '@material-ui/core/Typography';
import Avatar from '@material-ui/core/Avatar';
import Paper from '@material-ui/core/Paper';
import { withStyles } from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import TimelineTick from './TimelineTick.js'


function sortEvents(a,b) {
  if (moment(a.on).isBefore(moment(b.on)))
    return 1;
  if (moment(b.on).isBefore(moment(a.on)))
    return -1;
  return 0;
}

class Timeline extends React.Component {

  constructor(props) {
    super(props);
  }

  clickItem = (props) => {

    if(props.item != undefined)
    {
      var match = this.props.events.find(function(e) {
        return e.id == props.item;
      });

      if(match != undefined)
      {
        this.props.viewState(match);
      }
    }

  }


  render() {

    const { classes } = this.props;

    if(this.props.events.length == 0) return (<div></div>);

    var events = this.props.events.sort(sortEvents);
    var end = moment(events[events.length -1].on).startOf('hour');
    var start = moment(events[0].on).endOf('hour').add(1,'m');
    var current = moment(events[0].on).endOf('hour').add(1,'m');
    var ticks = [];
    var eventIndex = 0;

    while(current > end)
    {
       var desc =  '';

      // Format tick desc based on type of tick
       if(current.hour() == 0 && current.minute() == 0)
       {
         desc = current.format('MMMM Do');
       } else if(current.minute() == 0)
       {
         desc = current.format('h:mm a');
       } else
       {
         desc = current.format('h:mm');
       }

       // Initialize new tick
       var tick = {
         description: desc,
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

         console.log(current.format("dddd, MMMM Do YYYY, H:mm:ss a"));
         console.log(nextEventTime.format("dddd, MMMM Do YYYY, H:mm:ss a"));
         console.log(next.format("dddd, MMMM Do YYYY, H:mm:ss a"));

         if(current.isSameOrAfter(nextEventTime) && next.isBefore(nextEventTime))
         {
           console.log('MATCH');
            tick.events.push(events[eventIndex]);
            eventIndex ++;
         }
         else {
           console.log('NO MATCH');
           loopEvents = false;
         }
       }

       // Add and move on
       ticks.push(tick);

       current = next;
    }


    return (<div>
      {ticks.map(n => {
        return ( <TimelineTick
          viewState={this.props.viewState}
          events={n.events}
          description={n.description} />
        );
      })}
      </div>
    );
  }
}



export default (Timeline);
