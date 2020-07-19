public class CustCursor : Cursor<Program>
{
    public override Grid<Program> Grid => custUI.grid;
    public CustUI custUI;
}
