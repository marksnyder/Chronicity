import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import Typography from '@material-ui/core/Typography';
import RawEvents from './RawEvents.js'
import EventTimeline from './EventTimeline.js'


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
      value: 0
    };
  }

  handleChange = (event, value) => {
    this.setState({ value });
  };

  render() {
    const { classes } = this.props;
    const { value } = this.state;

    return  <div className={classes.root}>
        <AppBar position="static" color="default">
          <Tabs value={value} onChange={this.handleChange}>
            <Tab label="Visual" />
            <Tab label="Data" />
          </Tabs>
        </AppBar>
        {value === 0 && <TabContainer><EventTimeline events={this.props.events} classes={classes}  /></TabContainer>}
        {value === 1 && <TabContainer><RawEvents events={this.props.events} classes={classes} /></TabContainer>}
      </div>

  }
}

export default (TimelineView);
