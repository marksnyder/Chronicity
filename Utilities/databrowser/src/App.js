import React from 'react';
import PropTypes from 'prop-types';
import TimelineView from './components/timeline/View.js'
import FilterView from './components/filter/View.js'
import MenuView from './components/menu/View.js'
import { MuiThemeProvider, createMuiTheme,withStyles } from '@material-ui/core/styles';
import Chronicity from './clients/Chronicity.js'

const theme = createMuiTheme({
  palette: {
    primary: {
      // light: will be calculated from palette.primary.main,
      main: '#2e3336'
      // dark: will be calculated from palette.primary.main,
      // contrastText: will be calculated to contast with palette.primary.main
    },
    secondary: {
      main: '#35dcef'
    },
    // error: will use the default color
  },
});

const styles = theme => ({
  root: {
    flexGrow: 1,
    backgroundColor: theme.palette.background.paper,
  },
  flex: {
      flex: 1,
    }
});


class App extends React.Component {
  state = {
    view: 'timeline',
    filters: [ 'On.After=2015/01/01 01:02', 'Type=newthing' ],
    events: []
  };

  changeView = (view) => {
    this.setState({
      view: view
    });
  };

  applyFilters = (filters) => {

    this.setState({
      filters: filters,
      view: 'timeline'
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

    return (
    <MuiThemeProvider theme={theme}>
      <MenuView classes={classes} changeView={this.changeView} />
      {this.state.view == 'timeline' &&
        <TimelineView classes={classes} events={this.state.events} />
      }
      {this.state.view == 'filters' &&
         <FilterView classes={classes} filters={this.state.filters} applyFilters={this.applyFilters}  />
      }
    </MuiThemeProvider>
    );
  }
}


export default withStyles(styles)(App);
