import React from 'react';
import PropTypes from 'prop-types';
import Timeline from 'react-visjs-timeline';

const options = {
  width: '100%',
  height: '250px',
  stack: false,
  showMajorLabels: true,
  showCurrentTime: true,
  zoomMin: 1000000,
  type: 'background',
  format: {
    minorLabels: {
      minute: 'h:mma',
      hour: 'ha'
    }
  }
}

const items = [{
  start: new Date(2010, 7, 15),
  end: new Date(2010, 8, 2),  // end is optional
  content: 'Trajectory A',
}]

function EventTimeline(props) {
  const { classes } = props;

  return (
      <div><Timeline options={options}  items={items} /></div>
  );
}

EventTimeline.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default (EventTimeline);
