import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import ListItemText from '@material-ui/core/ListItemText';
import ListItem from '@material-ui/core/ListItem';
import List from '@material-ui/core/List';
import Divider from '@material-ui/core/Divider';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import CloseIcon from '@material-ui/icons/Close';
import Slide from '@material-ui/core/Slide';
import { withStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';

const styles = {
  appBar: {
    position: 'relative',
  },
  flex: {
    flex: 1,
  },
};

function Transition(props) {
  return <Slide direction="up" {...props} />;
}

class StateView extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {
    const { classes } = this.props;

    return  <div className={classes.root}>
      {this.props.data != null && <Dialog
        onClose={this.props.onClose}
        open={true} aria-labelledby="simple-dialog-title"
        fullScreen
        TransitionComponent={Transition}
        >
          <AppBar className={classes.appBar}>
            <Toolbar>
              <IconButton color="inherit" onClick={this.props.onClose} aria-label="Close">
                <CloseIcon />
              </IconButton>
              <Typography variant="title" color="inherit" className={classes.flex}>
                {this.props.data.on} Event: {this.props.data.type}
              </Typography>
            </Toolbar>
          </AppBar>
          <Paper className={classes.root} style={{ padding: 20 * 3 }}>
                <Table className={classes.table}>
                  <TableHead>
                    <TableRow>
                      <TableCell>Entity</TableCell>
                      <TableCell>Key</TableCell>
                      <TableCell>Value</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {this.props.data.items.map(n => {
                    return (
                      <TableRow key={n.key + n.entity}>
                        <TableCell component="th" scope="row">
                          {n.entity}
                        </TableCell>
                        <TableCell>{n.key}</TableCell>
                        <TableCell>{n.value}</TableCell>
                      </TableRow>
                    );
                  })}
                  </TableBody>
                </Table>
            </Paper>
    </Dialog>}
    </div>
  }
}

export default (StateView);
