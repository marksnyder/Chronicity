import React from 'react';
import Event from './Event.js'

class TimelineEvents extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {
    return (<div>
      {this.props.events.reverse().map(n => {
       return(<Event event={n}  />);
    })}
    </div> );

  }
}

export default (TimelineEvents);
