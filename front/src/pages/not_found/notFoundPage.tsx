import ButtonNotFound from "../../components/404/ButtonNotFound";

function PageNotFound(){

    return(
     <div style={{
         display: 'flex',
         flexDirection: 'column',
         justifyContent: 'center',
         alignItems: 'center',
         height: '100vh',
         textAlign: 'center',
         gap: '20px'
     }}>
        <h1>Page not found</h1>
        <ButtonNotFound />
     </div>
    )
}

export default PageNotFound;