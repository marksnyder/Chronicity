import React from 'react';
import PropTypes from 'prop-types';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';


function RawEvents(props) {
  const { classes } = props;

  return (
    <Paper className={classes.root}>
      <Table className={classes.table}>
        <TableHead>
          <TableRow>
            <TableCell>Identifier</TableCell>
            <TableCell>Type</TableCell>
            <TableCell>On</TableCell>
            <TableCell>Entities</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {props.events.map(n => {
            return (
              <TableRow key={n.id}>
                <TableCell>{n.id}</TableCell>
                <TableCell>{n.type}</TableCell>
                <TableCell>{n.on}</TableCell>
                <TableCell>
                  {n.entities.map(e => {
                    return <div key={e}><a href="#">{e}</a></div>
                  })}
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </Paper>
  );
}


export default (RawEvents);
