import React from 'react';
import PropTypes from 'prop-types';
import Timeline from 'react-visjs-timeline';

const options = {
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
  }
}


class EventTimeline extends React.Component {


  render() {

    const { classes } = this.props;

    var items = [];
    this.props.events.forEach(function(event) {
        items.push({
          start: event.on,
          content: event.type,
          id: event.id
        })
    });

    return (<div>
        {items.length > 0 &&
        <div><Timeline options={options} items={items} /></div>
        }
        {items.length == 0 &&
        <div>No Results To Display</div>
        }
      </div>
    );
  }
}



export default (EventTimeline);
