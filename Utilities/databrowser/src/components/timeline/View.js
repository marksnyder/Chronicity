import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import Typography from '@material-ui/core/Typography';
import Timeline from './Timeline.js'
import StateView from './StateView.js'
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
    this.state = {
      stateViewerData: null,
      focus: null
    };
  }

  closeStateViewer = () => {

    this.setState({
      stateViewerData: null
    });
  };

  openStateViewer = (data) => {
    data.items = [];

    this.setState({
      stateViewerData: data
    });

    var that = this;

     data.entities.forEach(function(e) {
      Chronicity.getEntityState(e,data.on)
      .then(function(data){
          that.addStateViewerItems(e,data);
      });
    });

  };

  addStateViewerItems(entity,items)
  {
    var data = this.state.stateViewerData;

    Object.keys(items).forEach(function(key) {
      data.items.push({
        entity: entity,
        key: key,
        value: items[key]
      });
    });

    this.setState({
      stateViewerData: data
    });

  }



  render() {
    const { classes } = this.props;
    const { tab } = this.state;

    return  <div className={classes.root}>
        <StateView
          data={this.state.stateViewerData}
          onClose={this.closeStateViewer}
          classes={classes}  />
          <Timeline
            events={this.props.events}
            stateChanges={this.props.stateChanges}
            viewState={this.openStateViewer}
            start={moment().startOf('hour')}
            end={moment().subtract(3, 'days')}
            classes={classes}  />
      </div>

  }
}

export default (TimelineView);
