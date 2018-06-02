import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import Typography from '@material-ui/core/Typography';
import RawEvents from './RawEvents.js'
import EventTimeline from './EventTimeline.js'
import FilterView from './FilterView.js'
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
      filters: [ 'On.After=2015/01/01 01:02', 'Type=newthing' ],
      events: []
    };
  }

  handleChange = (event, tab) => {
    this.setState({ tab: tab });
  };

  applyFilters = (filters) => {

    this.setState({
      filters: filters,
      tab: 0
    });

    var that = this;

    Chronicity.filterEvents(filters)
      .then((res) => { return res.json(); })
      .then((result) => {
          that.setState({
            events: result
          });
        }
      );

  };

  render() {
    const { classes } = this.props;
    const { tab } = this.state;

    return  <div className={classes.root}>
        <AppBar position="static" color="default">
          <Tabs value={tab} onChange={this.handleChange}>
            <Tab label="Visual" />
            <Tab label="Data" />
            <Tab label="Filters" />
          </Tabs>
        </AppBar>
        {tab === 0 && <TabContainer><EventTimeline events={this.state.events} classes={classes}  /></TabContainer>}
        {tab === 1 && <TabContainer><RawEvents events={this.state.events} classes={classes} /></TabContainer>}
        {tab === 2 && <TabContainer><FilterView classes={classes} filters={this.state.filters} applyFilters={this.applyFilters}  /></TabContainer>}
      </div>

  }
}

export default (TimelineView);
