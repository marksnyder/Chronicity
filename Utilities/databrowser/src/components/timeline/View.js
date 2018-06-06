import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import Typography from '@material-ui/core/Typography';
import RawEvents from './RawEvents.js'
import EventTimeline from './EventTimeline.js'
import CodeView from './CodeView.js'
import Chronicity from '../../clients/Chronicity.js'

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
      tab: 0,
      events: []
    };
  }

  handleChange = (event, tab) => {
    this.setState({ tab: tab });
  };

  applyData= (data) => {

    this.setState({
      events: data,
      tab: 0
    });

  };

  render() {
    const { classes } = this.props;
    const { tab } = this.state;

    return  <div className={classes.root}>
        <AppBar position="static" color="default">
          <Tabs value={tab} onChange={this.handleChange}>
            <Tab label="Visual" />
            <Tab label="Data" />
            <Tab label="Code" />
          </Tabs>
        </AppBar>
        {tab === 0 && <TabContainer><EventTimeline events={this.state.events} classes={classes}  /></TabContainer>}
        {tab === 1 && <TabContainer><RawEvents events={this.state.events} classes={classes} /></TabContainer>}
        {tab === 2 && <TabContainer><CodeView classes={classes} filters={this.state.filters} applyData={this.applyData}  /></TabContainer>}
      </div>

  }
}

export default (TimelineView);
