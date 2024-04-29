﻿namespace RiverBooks.Reporting.ReportEndpoints;

public record BookSalesResult(Guid BookId,
                              string Title,
                              string Author,
                              int Units,
                              decimal Sales)
{
  private BookSalesResult() : this(default!, default!, default!, default!, default!) { }
}


