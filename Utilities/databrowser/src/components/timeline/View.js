import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import Typography from '@material-ui/core/Typography';
import Timeline from './Timeline.js'
import Chronicity from '../../helpers/Chronicity.js'
import moment from 'moment';

function TabContainer(props) {
  return (
    <Typography component="div" style={{ padding: 8 * 3 }}>
      {props.children}
    </Typography>
  );
}

class TimelineView extends React.Component {

  constructor(props) {
    super(props);
  }


  render() {
    const { classes } = this.props;

    return  <div className={classes.root}>
          <Timeline
            events={this.props.events.reverse()}
            stateChanges={this.props.stateChanges}
            viewState={this.openStateViewer}
            start={moment().startOf('hour')}
            end={moment().subtract(3, 'days')}
            classes={classes}  />
      </div>

  }
}

export default (TimelineView);
