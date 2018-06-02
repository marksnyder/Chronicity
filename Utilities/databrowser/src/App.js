import React from 'react';
import PropTypes from 'prop-types';
import TimelineView from './components/timeline/View.js'
import MenuView from './components/menu/View.js'
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
      main: '#35dcef'
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
    view: 'timeline'
  };

  changeView = (view) => {
    this.setState({
      view: view
    });
  };


  render() {
    const { classes } = this.props;

    return (
    <MuiThemeProvider theme={theme}>
      <MenuView classes={classes} changeView={this.changeView} />
      {this.state.view == 'timeline' &&
        <TimelineView classes={classes} events={this.state.events} />
      }
    </MuiThemeProvider>
    );
  }
}


export default withStyles(styles)(App);
