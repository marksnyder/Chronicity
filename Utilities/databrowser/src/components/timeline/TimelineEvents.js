import React from 'react';
import Event from './Event.js'

class TimelineEvents extends React.Component {

  render() {
    return (<div>
      {this.props.events.reverse().map(n => {
       return(<Event event={n} key={n.id}  />);
    })}
    </div> );

  }
}

export default (TimelineEvents);
