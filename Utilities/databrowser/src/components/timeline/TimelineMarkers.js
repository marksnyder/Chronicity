import React from 'react';
import Marker from './Marker.js'

class TimelineMarkers extends React.Component {

  render() {
    return (<div>
      {this.props.markers.reverse().map(n => {
       return(<Marker marker={n} key={n.id}  />);
    })}
    </div> );

  }
}

export default (TimelineMarkers);
