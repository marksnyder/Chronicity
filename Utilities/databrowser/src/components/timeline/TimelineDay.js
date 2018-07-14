import React from 'react';
import TimelineHour from './TimelineHour.js'
import DataUtilities from '../../helpers/DataUtilities.js'

class TimelineGroup extends React.Component {

  render() {

    var groups = DataUtilities.groupByHour(this.props.group.markers, this.props.group.trackerChanges,this.props.group.start,this.props.group.end);

    return (<div>
      {groups.map(n => {
        return (<TimelineHour key={n.id}
          viewState={this.props.viewState}
          group={n} />
        );
      })}
      </div>
    );
  }
}



export default  (TimelineGroup);
