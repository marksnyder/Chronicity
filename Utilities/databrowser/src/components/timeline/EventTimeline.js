import React from 'react';
import PropTypes from 'prop-types';
import Timeline from 'react-visjs-timeline';



class EventTimeline extends React.Component {


  render() {

    const { classes } = this.props;

    const timelineData = {
      groups: [{ id:0, content:'Events'}],
      items: [],
      options: {
        width: '100%',
        height: '450px',
        stack: true,
        showMajorLabels: true,
        showCurrentTime: true,
        zoomMin: 1000000,
        type: 'point',
        format: {
          minorLabels: {
            minute: 'h:mma',
            hour: 'ha'
          }
        },
        groupOrder: 'content'
      }
    }


    this.props.events.forEach(function(event) {
        timelineData.items.push({
          start: event.on,
          content: event.type,
          id: event.id,
          group: 0
        })
    });


    for(var key in this.props.stateChanges) {
      if(this.props.stateChanges.hasOwnProperty(key)) { //to be safe

        var groupId =  timelineData.groups.length;
        timelineData.groups.push({ content: key, id: groupId });
        var groupItems = this.props.stateChanges[key];

        groupItems.forEach(function(stateChange) {
            if(stateChange.end != null)
            {
              timelineData.items.push({
                start: stateChange.start,
                end: stateChange.end,
                id: stateChange.entity + stateChange.key + stateChange.start + stateChange.end,
                type: 'background',
                className: 'timeline-' + stateChange.key ,
                group: groupId
              });
            }
        });

      }
    }


    return (<div>
        {timelineData.items.length > 0 &&
        <div><Timeline {...timelineData} /></div>
        }
        {timelineData.items.length == 0 &&
        <div>No Results To Display</div>
        }
      </div>
    );
  }
}



export default (EventTimeline);
