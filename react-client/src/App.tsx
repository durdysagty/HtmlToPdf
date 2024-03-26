import { ChangeEvent, DragEvent, useState } from 'react'
import Typography from '@mui/material/Typography'
import { Box, Button, List, ListItem, ListItemText } from '@mui/material'
import Grid from '@mui/material/Unstable_Grid2'
import { CloudUpload } from '@mui/icons-material'
import styled, { StyledComponent } from '@emotion/styled'

const VisuallyHiddenInput: StyledComponent<React.DetailedHTMLProps<React.InputHTMLAttributes<HTMLInputElement>, HTMLInputElement>> = styled('input')({
  clip: 'rect(0 0 0 0)',
  clipPath: 'inset(50%)',
  height: 1,
  overflow: 'hidden',
  position: 'absolute',
  bottom: 0,
  left: 0,
  whiteSpace: 'nowrap',
  width: 1,
});

function App() {

  const [fileNames, setFileNames] = useState<string[]>(['jkhgkjh', 'hkjhlkj'])

  const handleDragOver: (event: DragEvent<HTMLDivElement>) => void = (event) => {
    event.preventDefault()
  }

  const handleDrop: (event: DragEvent<HTMLDivElement>) => void = (event) => {
    event.preventDefault()
    setFile(event.dataTransfer?.files[0])
  }

  const handleFile: (event: ChangeEvent<HTMLInputElement>) => void = (event) => {
    if (event.target.files !== null)
      setFile(event.target.files[0])
  }

  const setFile: (file: File) => void = (file) => {
    if (process.env.NODE_ENV === "development")
      console.log(file.name)
    setFileNames(ps => ([...ps, file.name]))
    console.log(fileNames)
  }


  return (
    <Box bgcolor='lightgrey' height='100vh' textAlign='center'>
      <Box py={2}>
        <Typography variant="h3">HTML TO PDF CONVERTER</Typography>
        <Grid container>
          <Grid xs={6}>
            <Typography variant="h3">Add a file to convert</Typography>
            <Box onDrop={handleDrop} height='70vh' onDragOver={handleDragOver} bgcolor='white' m={4} borderRadius={4} sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <Box>
                <Button component="label" variant="contained" startIcon={<CloudUpload sx={{ width: '40px', height: '40px' }} />}>
                  <Typography variant="h4">Upload file</Typography>
                  <VisuallyHiddenInput type="file" onChange={handleFile} />
                </Button>
                <Typography variant="h6">or Drag and Drop any HTML</Typography>
              </Box>
            </Box>
          </Grid>
          <Grid xs={6}>
            <Typography variant="h3">Add a file to convert</Typography>
            <Box height='70vh' bgcolor='white' m={4} px={2} borderRadius={4} sx={{ display: 'flex', alignItems: 'start', justifyContent: 'start' }}>
              <List>
                {
                  fileNames.map(n => <ListItem key={n} sx={{ padding: 0 }} ><ListItemText primary={n} /></ListItem>)
                }
              </List>
            </Box>
          </Grid>
        </Grid>
      </Box>
    </Box>
  )
}

export default App
