using Grid;
public class CustCursor : Cursor<Program>
{
    public override Grid<Program> Grid => custUI.Grid;
    public CustUI custUI;
}
