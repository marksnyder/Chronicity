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
import TimelineGroup from './TimelineGroup.js'
import { Sticky, StickyContainer } from 'react-sticky';

function sortEvents(a,b) {
  if (moment(a.on).isBefore(moment(b.on)))
    return 1;
  if (moment(b.on).isBefore(moment(a.on)))
    return -1;
  return 0;
}


var headerStyle = {
  'background': '#35dcef',
  'text-align': 'center',
  'z-index': '100'
};


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

    var groups = [];

    var eventIndex = 0;

    while(current > end)
    {
       var desc =  '';

       // Initialize new tick
       var group = {
         description:  current.format('MMMM Do YY'),
         id: current.valueOf(),
         events: []
       };

       // Build the next tick time
       const next = current.clone().subtract(1, 'days');

       // Cycle through th enext events and add (assumes events are sorted by time desc)
       var loopEvents = true;

       while(loopEvents == true && eventIndex < events.length)
       {
         var nextEventTime = moment(events[eventIndex].on);

         if(current.isSameOrAfter(nextEventTime) && next.isBefore(nextEventTime))
         {
            group.events.push(events[eventIndex]);
            eventIndex ++;
         }
         else {
           loopEvents = false;
         }
       }

       // Add and move on
       groups.push(group);

       current = next;
    }

    if(group.length == 0) return (<div></div>);

    return (<div>
      {groups.map(n => {
        return (<div>
          <StickyContainer  className="container" key={n.id}>
            <Sticky>
              {({ style }) => (
                <div style={Object.assign(style,headerStyle)}>
                  <Typography variant="display1">
                    {n.description}
                  </Typography>
                </div>
              )}
            </Sticky>
          <TimelineGroup viewState={this.props.viewState} group={n}  />
          </StickyContainer>
        </div>
        );
      })}
      </div>
    );
  }
}



export default (Timeline);
