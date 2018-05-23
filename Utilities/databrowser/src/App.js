import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import { withStyles } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import MenuItem from '@material-ui/core/MenuItem';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';
import Divider from '@material-ui/core/Divider';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import MainPanel from './components/MainPanel.js'
import ObservationPanel from './components/ObservationPanel.js'


const drawerWidth = 240;

const styles = theme => ({
  root: {
    flexGrow: 1,
  },
  appFrame: {
    height:  `calc(100%)`,
    zIndex: 1,
    overflow: 'hidden',
    position: 'relative',
    display: 'flex',
    width: '100%',
  },
  appBar: {
    width: `calc(100% - ${drawerWidth}px)`,
  },
  'appBar-left': {
    marginLeft: drawerWidth,
  },
  'appBar-right': {
    marginRight: drawerWidth,
  },
  drawerPaper: {
    position: 'relative',
    width: drawerWidth,
  },
  toolbar: theme.mixins.toolbar,
  content: {
    flexGrow: 1,
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing.unit * 3,
  },
});

class App extends React.Component {
  state = {
    anchor: 'left',
  };

  handleChange = event => {
    this.setState({
      anchor: event.target.value,
    });
  };

  render() {
    const { classes } = this.props;

    return (
      <div className={classes.root}>
        <div className={classes.appFrame}>

          <Drawer
            variant="permanent"
            classes={{
              paper: classes.drawerPaper,
            }}  >
            <img src={require('./images/logo.png')} style={{width: 240}} />
            <Divider />
            <List>a</List>
            <Divider />
            <List>a</List>
          </Drawer>
          <main className={classes.content}>
             <Grid container spacing={24}>
                 <Grid item xs={12} sm={12}>
                    <Paper className={classes.paper}>
                        <MainPanel />
                    </Paper>
                  </Grid>
                  <Grid item xs={6} sm={6}>
                    <Paper className={classes.paper}>
                      Query Editor
                    </Paper>
                  </Grid>
                  <Grid item xs={6} sm={6}>
                    <Paper className={classes.paper}>
                      <ObservationPanel />
                    </Paper>
                  </Grid>
             </Grid>
          </main>

        </div>
      </div>
    );
  }
}

App.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(App);
