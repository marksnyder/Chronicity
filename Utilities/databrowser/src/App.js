import React from 'react';
import PropTypes from 'prop-types';
import TimelineView from './components/timeline/View.js'
import MenuView from './components/menu/View.js'
import CodeView from './components/codeedit/View.js'
import { MuiThemeProvider, createMuiTheme,withStyles } from '@material-ui/core/styles';

const theme = createMuiTheme({
  palette: {
    primary: {
      // light: will be calculated from palette.primary.main,
      main: '#2e3336'
      // dark: will be calculated from palette.primary.main,
      // contrastText: will be calculated to contast with palette.primary.main
    },
    secondary: {
      main: '#3498db'
    },
    // error: will use the default color
  },
});

const styles = theme => ({
  root: {
    flexGrow: 1,
    backgroundColor: theme.palette.background.paper
  },
  flex: {
      flex: 1,
  },
  margin: {
    margin: theme.spacing.unit
  }
});


class App extends React.Component {
  state = {
    view: 'code',
    events: [],
    stateChanges: []
  };

  changeView = (view) => {
    this.setState({
      view: view
    });
  };

  addStateChanges = (data,group) => {

    var allItems = this.state.stateChanges;

    if(allItems[group] == undefined) allItems[group] = [];

    var groupItems =  allItems[group].slice();
    var mergedGroupItems = groupItems.concat(data);
    allItems[group] = mergedGroupItems;

    this.setState({
      stateChanges: allItems
    });

  };

  addEvents = (data,iconStyle,initials) => {

    data.forEach(function(e){
      e.iconStyle = iconStyle;
      e.initials = initials;
    })

    var updated = this.state.events.slice();
    var merged = updated.concat(data);

    this.setState({
      events: merged
    });
  };

  clearEvents = () => {
    this.setState({
      events: []
    });
  };

  clearStateChanges = () => {
    this.setState({
      stateChanges: []
    });
  };


  render() {
    const { classes } = this.props;

    return (
    <MuiThemeProvider theme={theme}>
      <MenuView classes={classes} changeView={this.changeView} />
      {this.state.view == 'timeline' &&
        <TimelineView
          classes={classes}
          events={this.state.events} />
      }
      {this.state.view == 'code' &&
        <CodeView classes={classes}
          addEvents={this.addEvents}
          clearEvents={this.clearEvents}
          addStateChanges={this.addStateChanges}
          clearStateChanges={this.clearStateChanges} />
      }
    </MuiThemeProvider>
    );
  }
}


export default withStyles(styles)(App);
